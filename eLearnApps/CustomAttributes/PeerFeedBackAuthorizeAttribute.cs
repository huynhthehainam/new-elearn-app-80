using eLearnApps.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Web;
namespace eLearnApps.CustomAttribute
{
    public class PeerFeedBackAuthorizeAttribute : AuthorizeAttribute
    {
        public string Role { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(Role)) return false;

            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized) return false;
            var user = (UserModel)httpContext.Session[Constants.SessionUserKey];
            if (user == null) return false;

            if (Role.IndexOf(Constants.PeerFeedBackRoleNameAdmin, StringComparison.OrdinalIgnoreCase) > -1 || Role.IndexOf(Constants.PeerFeedBackRoleNameInstructor, StringComparison.OrdinalIgnoreCase) > -1)
            {
                return user.HasAdmin ? user.HasAdmin : user.IsInstructor;
            }
            if (Constants.PeerFeedBackRoleNameStudent.Equals(Role, StringComparison.OrdinalIgnoreCase))
                return user.IsStudent;
            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "Error",
                        action = "AccessDenied"
                    })
            );
        }
    }
}