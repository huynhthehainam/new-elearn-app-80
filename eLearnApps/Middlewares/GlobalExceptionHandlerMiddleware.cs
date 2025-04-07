using eLearnApps.Business.Interface;
using eLearnApps.Entity.Logging;
using eLearnApps.Helpers;
using System.Security.Claims;
using System.Text;

namespace eLearnApps.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next,
                                                 IServiceScopeFactory serviceScopeFactory,
                                                 IConfiguration configuration,
                                                 IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                log.Error("Exception occurred.", ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                log.Error("Exception occurred.", exception);

                // Resolve IErrorLogService within the scope
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var errorLogService = scope.ServiceProvider.GetRequiredService<IErrorLogService>();
                    var errorLogId = LogException(context, exception, errorLogService);
                    RedirectToErrorPage(context, exception, errorLogId);
                }
            }
            catch (Exception ex)
            {
                // just in case support function throw exception, we let it be handled by web.config customerrors
                log.Error("Exception occurred.", ex);
            }
        }

        public int GetCourseId(HttpContext context)
        {
            string queryString = context.Request.Query["courseId"].ToString();

            if (string.IsNullOrEmpty(queryString))
            {
                throw new Exception("elearnapps access must be accompanied with course Id");
            }
            return Convert.ToInt32(queryString);
        }

        private void RedirectToErrorPage(HttpContext context, Exception exception, string errorLogId)
        {
            context.Response.Clear();

            if (exception is BadHttpRequestException || context.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                var redirectUrl = "/Error/Sorry";

                if (IsAjaxRequest(context))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json; charset=utf-8";

                    var jsonReturn = new
                    {
                        redirectUrl,
                        errorLogId,
                        exception = exception.Message
                    };

                    context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(jsonReturn));
                }
                else
                {
                    context.Response.Redirect(redirectUrl);
                }
            }
            else
            {
                var routeValues = new RouteValueDictionary
                {
                    ["controller"] = "Error",
                    ["action"] = "Sorry",
                    ["messages"] = exception.ToString(),
                    ["errorLogId"] = errorLogId
                };

                context.Response.Redirect($"/Error/Sorry?errorLogId={errorLogId}");
            }
        }

        private bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        protected string LogException(HttpContext context, Exception exception, IErrorLogService errorLogService)
        {
            if (exception is ThreadAbortException)
                return string.Empty;

            int userId = int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 0;
            var courseId = GetCourseId(context);
            var claimHelper = new ClaimHelper(_serviceProvider);
            var enrollment = claimHelper.GetEnrollmentFromCache(userId, courseId);
            var roleId = enrollment?.RoleId ?? 0;
            var webHelper = new WebHelper(context);
            var errorLog = new ErrorLog
            {
                UserId = userId,
                ToolId = webHelper.CategoryName,
                OrgUnitId = courseId,
                ToolAccessRoleId = roleId,
                ErrorPage = webHelper.GetThisPageUrl(true),
                ErrorMessage = exception.Message,
                ErrorDetails = exception.ToString(),
                ErrorTime = DateTime.UtcNow,
                IpAddress = webHelper.GetCurrentIpAddress(),
                SessionId = context.Session.GetString("SessionId") ?? string.Empty
            };

            var inserted = errorLogService.Insert(errorLog);
            return inserted.ToString();
        }
    }

}
