using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using eLearnApps.Helpers;
using eLearnApps.Business.Interface;

namespace eLearnApps.CustomAttribute
{
    public class ClaimRequirementAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _systemName;

        public ClaimRequirementAttribute(params string[] systemName)
        {
            _systemName = systemName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var serviewProvider = context.HttpContext.RequestServices.GetRequiredService<IServiceProvider>();
            var configuration = serviewProvider.GetRequiredService<IConfiguration>();
            var httpContextAccessor = serviewProvider.GetRequiredService<IHttpContextAccessor>();
            var claimHelper = new ClaimHelper(serviewProvider);
            var isAuthorized = false;

            var httpContext = context.HttpContext;
            var webHelper = new WebHelper(httpContext);

            // Get access denied URL
            var accessDeniedUrl = $"/Error/AccessDenied?returnUrl={webHelper.GetUrlReferrer()}";

            if (httpContext.Request.Query.ContainsKey("courseId"))
            {
                var id = httpContext.Request.Query["courseId"];
                if (int.TryParse(id, out var courseId))
                {
                    var user = httpContext.User as ClaimsPrincipal;
                    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Check if cache for this user is still valid
                    var userInfo = claimHelper.GetUserInfoFromCache(userId);
                    if (userInfo == null)
                        claimHelper.SetUserInfoIntoCache(userId);

                    // Process claims via cache data
                    isAuthorized = false;
                    var enrollment = claimHelper.GetEnrollmentFromCache(userId, courseId);
                    if (enrollment != null)
                    {
                        var roleId = enrollment.RoleId;
                        var permissions = claimHelper.GetPermissionCache(roleId);
                        var permission = permissions.FirstOrDefault(p => _systemName.Contains(p.SystemName));

                        if (permission != null)
                        {
                            isAuthorized = true;
                        }
                        else
                        {
                            // TODO: Redirect to next priority URL
                        }
                    }
                }
            }

            // Access denied path
            if (!isAuthorized)
            {
                if (httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new { redirectUrl = accessDeniedUrl }) { StatusCode = StatusCodes.Status500InternalServerError };
                }
                else
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", new { returnUrl = webHelper.GetUrlReferrer() });
                }
            }
        }
    }
}
