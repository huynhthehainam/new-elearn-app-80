using Microsoft.AspNetCore.Mvc.Filters;


namespace eLearnApps.CustomAttribute
{
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var env = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

            // Assume session is enabled and contains a SessionID
            var sessionId = context.HttpContext.Session.GetString("SessionID");
            if (string.IsNullOrEmpty(sessionId)) return;

            var relativePath = Path.Combine("Content", "Upload", "EE", sessionId);
            var absolutePath = Path.Combine(env.WebRootPath, relativePath);

            if (Directory.Exists(absolutePath))
            {
                Directory.Delete(absolutePath, recursive: true);
            }
        }
    }
}