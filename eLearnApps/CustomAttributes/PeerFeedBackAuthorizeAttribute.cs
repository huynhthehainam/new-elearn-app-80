using eLearnApps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text.Json;
using System.Web;
namespace eLearnApps.CustomAttribute
{
    public class PeerFeedBackAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string Role { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var constants = new Constants(configuration);
            if (string.IsNullOrEmpty(Role))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "AccessDenied" }
                });
                return;
            }

            var sessionData = httpContext.Session.GetString(constants.SessionUserKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "AccessDenied" }
                });
                return;
            }

            var user = JsonSerializer.Deserialize<UserModel>(sessionData);

            if (user == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "AccessDenied" }
                });
                return;
            }

            bool isAuthorized = false;

            if (Role.Contains(constants.PeerFeedBackRoleNameAdmin, StringComparison.OrdinalIgnoreCase) ||
                Role.Contains(constants.PeerFeedBackRoleNameInstructor, StringComparison.OrdinalIgnoreCase))
            {
                isAuthorized = user.HasAdmin || user.IsInstructor;
            }
            else if (Role.Equals(constants.PeerFeedBackRoleNameStudent, StringComparison.OrdinalIgnoreCase))
            {
                isAuthorized = user.IsStudent;
            }

            if (!isAuthorized)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "AccessDenied" }
                });
            }
        }
    }
}