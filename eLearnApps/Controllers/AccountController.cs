using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Logging.Dto;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using eLearnApps.Helpers;
using Microsoft.AspNetCore.Http;
using System.Web;
using eLearnApps.Models;

namespace eLearnApps.Controllers
{
    public class AccountController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<char> sAllowed = new List<char>("1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.{}:\\\"");

        private int _courseId;
        private string? _toolName;
        private readonly IAppSettingService _appSettingService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IValenceService _valenceService;
        private readonly IRoleService _roleService;
        private readonly IServiceProvider _serviewProvider;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPeerFeedbackService _peerFeedbackService;
        private readonly WebHelper _webHelper;
        public AccountController(IAppSettingService appSettingService, IConfiguration configuration, IUserService userService, IValenceService valenceService, IRoleService roleService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviewProvider, ILoggingService loggingService, IPeerFeedbackService peerFeedbackService)
        {
            _appSettingService = appSettingService;
            _configuration = configuration;
            _userService = userService;
            _valenceService = valenceService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = new WebHelper(httpContextAccessor);
            _serviewProvider = serviewProvider;
            _loggingService = loggingService;
            _peerFeedbackService = peerFeedbackService;
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
        [AllowAnonymous]
        public ActionResult PFLTI()
        {
            var showLoginPage = _appSettingService.GetByKey("ShowLoginPage");
            if (showLoginPage.Value.ToLower() == "true")
            {
                return View();
            }
            var domain = _configuration.GetValue<string>("Domain");
            return Redirect($"https://{domain}");
        }
        [HttpPost, AllowAnonymous]
        public ActionResult Lti(string returnUrl)
        {
            LogDebug("account-lti", $"landing: {returnUrl}");
            var contextId = Request.Form["context_id"];
            var userIdString = Request.Form["user_id"];
            var constants = new Constants(_configuration);
            var routeValue = new
            {
                ReturnUrl = $"{constants.HomePageUrl}",
                Content = "You don't have permission access"
            };
            var userId = ExtractUserId(userIdString);
            var courseId = Convert.ToInt32(contextId);
            _courseId = courseId;

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
            var claimHelper = new ClaimHelper(_serviewProvider);
            log.Debug($"Setting permission into cache: {userId} - {courseId}");
            claimHelper.SetUserInfoIntoCache(user);
            var enrollment = claimHelper.GetEnrollmentFromCache(userId, courseId);

            log.Debug($"Get redirection url {userId} - {courseId}");
            var userInfo = claimHelper.GetUserInfoFromCache(userId);

            log.Debug($"Setting permission into cache: {userId} - {courseId}");

            if (enrollment != null)
            {
                var roleId = enrollment.RoleId;
                var reload = true;
                // this is login stage, so we reload when when ever passing this stage
                var topPermission = claimHelper.GetPermissionCache(roleId, reload)
                    .Where(p => string.Equals(p.Category, toolName, StringComparison.InvariantCultureIgnoreCase))
                    .OrderBy(p => p.Order).FirstOrDefault();
                if (topPermission != null)
                {
                    var targetUrl = string.Format(topPermission.Url, enrollment.CourseId);
                    var sessionId = HttpContext.Session.Id;
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
                        QueryString = JsonSerializer.Serialize(Request.QueryString),
                        ReferrerUrl = Request.Headers["Referer"].ToString(),
                        SessionId = HttpContext.Session.Id
                    });
                    return Redirect(targetUrl);
                }
            }

            log.Debug($"no access found for user: {userId} - {courseId}");
            return RedirectToAction("AccessDenied", "Error");
        }
        [HttpPost, AllowAnonymous]
        public ActionResult PFLTI(string returnUrl)
        {
            var constants = new Constants(_configuration);
            var claimHelper = new ClaimHelper(_serviewProvider);
            LogDebug("account-PFLTI", $"landing: {returnUrl}");
            var userIdString = Request.Form["user_id"];

            var routeValue = new
            {
                ReturnUrl = $"{constants.HomePageUrl}",
                Content = "You don't have permission access - PFLTI"
            };
            var userId = ExtractUserId(userIdString);

            LogDebug("account-lti", $"landing on debug with {userId}");

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
                LogDebug("account-PFLTI", $"Unable to find user: {userId}.");
                return RedirectToAction("AccessDenied", "Error", routeValue);
            }

            _toolName = Category.PeerFeedback.ToString();
            log.Debug($"Signing in user: {userId}");
            SignIn(user);

            log.Debug($"Setting PFLTI access into cache: {userId}");
            claimHelper.SetPFAccessIntoCache(user);
            var sessionData = HttpContext.Session.GetString(constants.SessionUserKey);
            UserModel? userInfo = null;

            if (!string.IsNullOrEmpty(sessionData))
            {
                userInfo = JsonSerializer.Deserialize<UserModel>(sessionData);
            }

            log.Debug($"Get redirection url {userId}");

            var sessions = _peerFeedbackService.PeerFeedbackSessionsGetList();
            sessions = (from session in sessions
                        from enrolled in userInfo.CurrentLoadedCourses
                        where (session.CourseOfferingCode ?? string.Empty).Contains(enrolled.CourseCode ?? string.Empty)
                        select session).ToList();
            if (!userInfo.HasAdmin && userInfo.IsInstructor && sessions.Count == 0)
            {
                return RedirectToAction("NoSurvey", "Error", routeValue);
            }

            if (!userInfo.HasAdmin && userInfo.IsStudent && sessions.Count == 0)
            {
                return RedirectToAction("NoSurvey", "Error", routeValue);
            }

            if (userInfo.CurrentLoadedCourses != null && userInfo.CurrentLoadedCourses.Any())
            {
                HttpContext.Session.SetString("IsInitialLanding", "true");
                return RedirectToAction("Index", "PeerFeedback");
            }

            log.Debug($"no access found for user: {userId}");
            return RedirectToAction("AccessDenied", "Error");
        }
        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateClaims(int courseId, string hash, string referrer)
        {
            // TODO, need to just integrate with claims, no need to use this function
            var claimHelper = new ClaimHelper(_serviewProvider);
            var user = User as ClaimsPrincipal;
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var enrollment = claimHelper.GetEnrollmentFromCache(userId, courseId);
            if (enrollment != null)
            {
                var roleId = enrollment.RoleId;
                var permissions = claimHelper.GetPermissionCache(roleId);

                var categoryName = string.IsNullOrEmpty(referrer) ? _webHelper.CategoryName : _webHelper.GetCategoryNameFromUrl(referrer);
                var nextPriorityPath = permissions.Where(p => p.Category == categoryName).OrderBy(p => p.Order).FirstOrDefault();
                if (nextPriorityPath != null)
                {
                    return Json(new
                    {
                        Data = new
                        {
                            redirectUrl = HttpUtility.HtmlEncode(string.Format(nextPriorityPath.Url, courseId)),
                            hash = HttpUtility.HtmlEncode("#" + hash)
                        }
                    });
                }
            }

            // user have not login, redirect 
            return Json(new
            {
                Data = new
                {
                    redirectUrl = HttpUtility.HtmlEncode(Url.Action("AccessDenied", "Error", Request.Scheme)),
                    hash = HttpUtility.HtmlEncode("#" + hash)
                }
            });
        }
        private async Task SignIn(User user)
        {
            var roles = _roleService.GetRoleByUserId(user.Id);
            var roleName = string.Join(",", roles.Select(x => x.Name));

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.DisplayName),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(Core.Constants.CourseIdClaim, _courseId.ToString()),
        new Claim(Core.Constants.ToolNameClaim, _toolName),
        new Claim(Core.Constants.RoleClaim, roleName),
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties { IsPersistent = false });
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

            var debugMessage = JsonSerializer.Serialize(logItem);
            debugMessage = FilterWhitelist(debugMessage);

            // Log4Net
            log.Debug(debugMessage);
        }
        private string FilterWhitelist(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            string result = string.Concat(fileName.Where(c => sAllowed.Contains(c)));
            return result;
        }
    }
}
