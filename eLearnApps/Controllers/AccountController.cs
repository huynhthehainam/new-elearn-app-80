using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Entity.Logging;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLearnApps.Controllers
{
    public class AccountController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int _courseId;
        private string? _toolName;
        private readonly IAppSettingService _appSettingService;
        private readonly IConfiguration _configuration;
        public AccountController(IAppSettingService appSettingService, IConfiguration configuration)
        {
            _appSettingService = appSettingService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        public IActionResult LtiView()
        {
            var showLoginPage = _appSettingService.GetByKey("ShowLoginPage");
            if (showLoginPage.Value.ToLower() == "true")
            {
                return View("Lti");
            }
            else
            {
                var domain = _configuration.GetValue<string>("Domain");
                return Redirect($"https://{domain}");
            }
        }
        [HttpPost, AllowAnonymous]
        public ActionResult Lti(string returnUrl)
        {
            LogDebug("account-lti", $"landing: {returnUrl}");
            var contextId = Request.Form["context_id"];
            var userIdString = Request.Form["user_id"];

            var routeValue = new
            {
                ReturnUrl = $"{Constants.HomePageUrl}",
                Content = "You don't have permission access"
            };
            var userId = ExtractUserId(userIdString);
            var courseId = Convert.ToInt32(contextId);
            _courseId = courseId;
            var constants = new Constants(_configuration);
            LogDebug("account-lti", $"landing on debug with {userId} - {courseId}");

            // if debug, and requestUrl is not set, means we are debuging with LTI
            if (string.Equals(constants.ValidateLTI, "true", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!ValidateLTI())
                    return RedirectToAction("AccessDenied", "Error", routeValue);
            }

            // validate userId
            var user = _userService.GetById(userId);
            if (user == null)
            {
                LogDebug("account-lti", $"Unable to find user: {userId}.");
                return RedirectToAction("AccessDenied", "Error", routeValue);
            }

            // validation done
            // sync enrollment for this user/course first. just incase this is a recent enrollment in D2L
            log.Debug($"Sync enrollment: {userId} - {courseId}");
            _valenceService.SyncEnrollment(userId, courseId);

            string toolName = Request.Form["custom_customtool"].ToString().ToLower();
            if (string.IsNullOrWhiteSpace(toolName))
                toolName = returnUrl.Split("/".ToCharArray()).Where(str => !string.IsNullOrWhiteSpace(str)).FirstOrDefault().ToLower();
            _toolName = toolName;
            log.Debug($"Signing in user: {userId} - {courseId}");
            SignIn(user);

            // set userinfo into cache
            log.Debug($"Setting permission into cache: {userId} - {courseId}");
            ClaimHelper.SetUserInfoIntoCache(user);
            var enrollment = ClaimHelper.GetEnrollmentFromCache(userId, courseId);

            log.Debug($"Get redirection url {userId} - {courseId}");
            var userInfo = ClaimHelper.GetUserInfoFromCache(userId);

            log.Debug($"Setting permission into cache: {userId} - {courseId}");

            if (enrollment != null)
            {
                var roleId = enrollment.RoleId;
                var reload = true;
                // this is login stage, so we reload when when ever passing this stage
                var topPermission = ClaimHelper.GetPermissionCache(roleId, reload)
                    .Where(p => string.Equals(p.Category, toolName, StringComparison.InvariantCultureIgnoreCase))
                    .OrderBy(p => p.Order).FirstOrDefault();
                if (topPermission != null)
                {
                    var targetUrl = string.Format(topPermission.Url, enrollment.CourseId);

                    log.Debug($"target url: {userId} - {courseId} - {targetUrl}");
                    _loggingService.Debug(new DebugLog
                    {
                        Browser = Request.Headers["User-Agent"].ToString(),
                        CreateOn = DateTime.UtcNow,
                        FullMessage = $"target url: {userId} - {courseId} - {targetUrl}",
                        ShortMessage = $"target url: {userId} - {courseId} - {targetUrl}",
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        OrgUnitId = courseId,
                        UserId = userId,
                        PageUrl = Request.Path,
                        QueryString = JsonConvert.SerializeObject(Request.QueryString),
                        ReferrerUrl = Request.Headers["Referer"].ToString(),
                        SessionId = HttpContext.Session.Id
                    });
                    return Redirect(targetUrl);
                }
            }

            log.Debug($"no access found for user: {userId} - {courseId}");
            return RedirectToAction("AccessDenied", "Error");
        }
        private int ExtractUserId(string userIdString)
        {
            if (!string.IsNullOrEmpty(userIdString))
            {
                if (userIdString.IndexOf("_", StringComparison.Ordinal) > -1)
                {
                    var arr = userIdString.Split('_');
                    return Convert.ToInt32(arr[1]);
                }
            }

            return 0;
        }
        private bool ValidateLTI()
        {
            var ConsumerSecret = _configuration.GetValue<string>("AuthenticationSecret");
            var ConsumerKey = _configuration.GetValue<string>("AuthenticationKey");
            var signatureHost = _configuration.GetValue<string>("Signature_Host");
            var signatureScheme = _configuration.GetValue<string>("Signature_Scheme");
            var timestampDiffAllowance = _configuration.GetValue<int>("TimestampAllowance");

            string incomingSignature = Request.Form["oauth_signature"];
            string incomingTimestamp = Request.Form["oauth_timestamp"];

            var auth = new Models.D2L.OAuth();
            string Signature = auth.GenerateSignature(HttpContext.Request, ConsumerSecret, signatureHost, signatureScheme);

            DateTime UnixEpoch = new DateTime(1970, 1, 1);
            var timeStampUtc = UnixEpoch.AddSeconds(Convert.ToInt64(incomingTimestamp));
            var timeDiff = (DateTime.UtcNow - timeStampUtc).TotalSeconds;

            if (timeDiff > timestampDiffAllowance) return false;
            if (Signature != incomingSignature) return false;

            return true;
        }
        private void LogDebug(string actioncategory, string actionDescription)
        {
            var logItem = new Logging
            {
                Audit = new AuditLog
                {
                    UserId = 0,
                    OrgUnitId = 0,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    ToolId = "elearnapps",
                    ToolAccessRoleId = 0,
                    ActionCategory = actioncategory,
                    ActionDescription = actionDescription,
                    ActionTime = DateTime.UtcNow
                }
            };

            var debugMessage = JsonConvert.SerializeObject(logItem);
            debugMessage = FilterWhitelist(debugMessage);

            // Log4Net
            log.Debug(debugMessage);
        }
    }
}
