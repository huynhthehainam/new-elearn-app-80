using eLearnApps.Business.Interface;
using eLearnApps.Controllers;
using eLearnApps.Core;
using eLearnApps.Extension;
using eLearnApps.ViewModel.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace eLearnApps.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TrackingLogAttribute : ActionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public TrackingLogAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ILoggingService loggingService = context.HttpContext.RequestServices.GetRequiredService<ILoggingService>();
            var controllerType = context.Controller.GetType().BaseType?.Name ?? "Unknown";
            var serviceScopeFactory = context.HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();
            try
            {
                var request = context.HttpContext.Request;
                var routeValues = context.RouteData.Values;
                var controllerName = routeValues.ContainsKey("controller") ? routeValues["controller"]?.ToString() : "Unknown";
                var actionName = routeValues.ContainsKey("action") ? routeValues["action"]?.ToString() : "Unknown";
                var roleId = 0;

                LoggingModel loggingModel = null;

                if (controllerType == "BaseController" && context.Controller is BaseController controller)
                {
                    var courseId = controller.CourseId;
                    var userInfo = controller.UserInfo;

                    var enrollment = userInfo.CurrentLoadedCourses.FirstOrDefault(c => c.CourseId == courseId);
                    if (enrollment != null)
                        roleId = enrollment.RoleId;

                    loggingModel = new LoggingModel
                    {
                        UserId = userInfo.UserId,
                        OrgUnitId = courseId,
                        RoleId = roleId,
                        IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                        ToolId = $"{controllerName}_{actionName}",
                        UserName = string.IsNullOrEmpty(userInfo.UserName) ? "Unknown" : userInfo.UserName,
                        SessionId = context.HttpContext.Session.Id
                    };
                }
                else if (controllerType == "BaseNoCourseController" && context.Controller is BaseNoCourseController baseController)
                {
                    var userInfo = baseController.UserInfo;

                    loggingModel = new LoggingModel
                    {
                        UserId = userInfo.UserId,
                        RoleId = roleId,
                        IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                        ToolId = $"{controllerName}_{actionName}",
                        UserName = string.IsNullOrEmpty(userInfo.UserName) ? "Unknown" : userInfo.UserName,
                        SessionId = context.HttpContext.Session.Id
                    };
                }

                if (loggingModel != null)
                {
                    Task.Run(async () =>
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var loggingService = scope.ServiceProvider.GetRequiredService<ILoggingService>();
                            log.Info($"Action Called: {JsonSerializer.Serialize(loggingModel)}");
                            loggingService.Log(LogType.TrackLog, loggingModel);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("************************ Exception on logging.", ex);
            }

            base.OnActionExecuting(context);
        }
    }
}
