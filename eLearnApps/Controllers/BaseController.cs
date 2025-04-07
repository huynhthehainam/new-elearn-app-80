using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using eLearnApps.Business.Interface;
using eLearnApps.Core.Caching;
using eLearnApps.CustomAttribute;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Security;
using eLearnApps.Helpers;
using eLearnApps.Models;
using System.Reflection;
using eLearnApps.Entity.Valence;

namespace eLearnApps.Controllers
{
    [Authorize, TrackingLog]
    public abstract class BaseController : Controller
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        private readonly IErrorLogService _errorLogService;
        protected readonly ICacheManager _cacheManager;
        protected readonly WebHelper _webHelper;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly Constants _constants;
        private readonly IServiceProvider _serviewProvider;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaseController(
            ICacheManager cacheManager,
            IErrorLogService errorLogService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IServiceProvider serviewProvider,
            ICompositeViewEngine viewEngine)
        {
            _configuration = configuration;
            _serviewProvider = serviewProvider;
            _httpContextAccessor = httpContextAccessor;
            _constants = new Constants(configuration);
            _cacheManager = cacheManager;
            _errorLogService = errorLogService;
            _webHelper = new WebHelper(httpContextAccessor);
            _viewEngine = viewEngine;
        }

        public int CourseId
        {
            get
            {
                if (!int.TryParse(HttpContext.Request.Query["courseId"], out int courseId))
                {
                    throw new Exception("eLearnApps access must be accompanied with courseId");
                }
                return courseId;
            }
        }

        public UserModel UserInfo
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return _cacheManager.Get<UserModel>(string.Format(_constants.KeyUserInfo, userId));
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

        public string MenuName
        {
            get
            {
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;

                return controllerName switch
                {
                    nameof(FunctionName.Ffts) => PartialMenuViewName._LeftMenu.ToString(),
                    nameof(FunctionName.InClassSensing) => PartialMenuViewName._LeftMenuIcs.ToString(),
                    nameof(FunctionName.Security) => PartialMenuViewName._LeftMenuAdmin.ToString(),
                    nameof(FunctionName.Cmt) => PartialMenuViewName._LeftMenuCMT.ToString(),
                    nameof(FunctionName.Journal) => PartialMenuViewName._LeftMenuJournal.ToString(),
                    nameof(FunctionName.Pet) => PartialMenuViewName._LeftMenuPet.ToString(),
                    nameof(FunctionName.RPT) => PartialMenuViewName._LeftMenuRPT.ToString(),
                    nameof(FunctionName.Ee) => PartialMenuViewName._LeftMenuEE.ToString(),
                    _ => string.Empty
                };
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Query.TryGetValue("courseId", out var courseIdValue) &&
                int.TryParse(courseIdValue, out int courseId))
            {
                ViewBag.CourseId = courseId;
                ViewBag.TermId = UserInfo?.CurrentLoadedCourses.FirstOrDefault(e => e.CourseId == courseId)?.SemesterId;
            }

            ViewBag.GitHash = GitHash;
        }



        public virtual string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.ActionDescriptor.ActionName;
            }

            ViewData.Model = model;

            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"View '{viewName}' not found.");
            }

            var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());
            viewResult.View.RenderAsync(viewContext).Wait();

            return sw.ToString();
        }

        private void RedirectToErrorPage(ExceptionContext context, string errorLogId)
        {
            context.ExceptionHandled = true;
            var httpException = context.Exception as Exception;

            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(new
                {
                    redirectUrl = Url.Action("Sorry", "Error"),
                    ErrorLogId = errorLogId,
                    Exception = context.Exception.Message
                });
            }
            else
            {
                context.Result = RedirectToAction("Sorry", "Error", new { errorLogId });
            }
        }

        protected string LogException(ExceptionContext context)
        {
            if (context.Exception is ThreadAbortException)
                return string.Empty;

            var exception = context.Exception;
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var claimHelper = new ClaimHelper(_serviewProvider);
            var enrollment = claimHelper.GetEnrollmentFromCache(UserInfo.UserId, CourseId);
            var roleId = enrollment?.RoleId ?? 0;

            var errorLog = new ErrorLog
            {
                UserId = UserInfo.UserId,
                ToolId = _webHelper.CategoryName,
                OrgUnitId = CourseId,
                ToolAccessRoleId = roleId,
                ErrorPage = _webHelper.GetThisPageUrl(true),
                ErrorMessage = exception.Message,
                ErrorDetails = exception.ToString(),
                ErrorTime = DateTime.UtcNow,
                IpAddress = ip,
                SessionId = HttpContext.Session.Id
            };

            return _errorLogService.Insert(errorLog).ToString();
        }
    }
}
