using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using eLearnApps.Business.Interface;
using eLearnApps.Core.Caching;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Security;
using eLearnApps.Helpers;
using eLearnApps.Models;
using eLearnApps.CustomAttribute;
using System.Threading;
using eLearnApps.Extension;
using System.Reflection;

namespace eLearnApps.Controllers
{
    [Authorize, TrackingLog, SessionExpire]
    public abstract class BaseNoCourseController : Controller
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IErrorLogService _errorLogService;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ICacheManager _cacheManager;
        protected readonly WebHelper _webHelper;

        public UserModel UserInfo => HttpContext.Session.GetObject<UserModel>("UserInfo");

        public string MenuName
        {
            get
            {
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
                return string.Equals(controllerName, FunctionName.PeerFeedback.ToString(), StringComparison.OrdinalIgnoreCase)
                    ? PartialMenuViewName._LeftMenuPF.ToString()
                    : string.Empty;
            }
        }

        public string GitHash
        {
            get
            {
                return Assembly.GetExecutingAssembly()
           .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
           .InformationalVersion.Split('+')[1] ?? "Unknown";
            }
        }

        public BaseNoCourseController(ICacheManager cacheManager, IErrorLogService errorLogService, IServiceProvider serviceProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService)
        {
            _cacheManager = cacheManager;
            _errorLogService = errorLogService;
            _currentUserService = currentUserService;
            _webHelper = new WebHelper(httpContextAccessor);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.HttpContext.Request.Query.TryGetValue("courseId", out var courseIdStr) && int.TryParse(courseIdStr, out var courseId))
            {
                ViewBag.CourseId = courseId;
                ViewBag.TermId = UserInfo?.CurrentLoadedCourses.FirstOrDefault(e => e.CourseId == courseId)?.SemesterId;
            }

            ViewBag.GitHash = GitHash;
        }



        private void RedirectToErrorPage(ExceptionContext context, string errorLogId)
        {
            context.ExceptionHandled = true;
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.HttpContext.Response.StatusCode = 400;
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    redirectUrl = Url.Action("Sorry", "Error")
                }));
            }
            else
            {
                context.Result = RedirectToAction("Sorry", "Error", new { messages = context.Exception.ToString(), errorLogId });
            }
        }


    }
}
