using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace eLearnApps.CustomAttribute
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            IConfiguration configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var constants = new Constants(configuration);
            var session = httpContext.Session;

            if (session == null || session.GetString(constants.SessionUserKey) == null)
            {
                context.Result = new RedirectToActionResult("PFLTI", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
