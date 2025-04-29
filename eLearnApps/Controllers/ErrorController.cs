using eLearnApps.Extension;
using eLearnApps.Models;
using Microsoft.AspNetCore.Mvc;

namespace eLearnApps.Controllers
{
    public class ErrorController : Controller
    {
        private readonly Constants _constants;
        public ErrorController(IConfiguration configuration)
        {
            _constants = new Constants(configuration);
        }
        public ActionResult Index(int type = 0)
        {
            // TODO need better handling
            bool IsMergeSection = false;
            if (type == 1)
            {
                IsMergeSection = true;
                ViewBag.Title = "RPT";
                ViewBag.Message = "eLearnApps is not able to process result for Non ISIS courses and ISIS merge courses for the moment. Please process and submit result using corresponding individual course.";
            }
            else if (type == 2)
            {
                IsMergeSection = true;
                ViewBag.Title = "RPT";
                ViewBag.Message = "eLearnApps is currently not able to support non ISIS and ISIS Merge Courses. Please visit corresponding individual course to view your results.";
            }
            else
            {
                ViewBag.Title = "eLearnApps - Error";
            }

            ViewBag.IsMergeSections = IsMergeSection;

            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public IActionResult Sorry(string messages, string errorLogId)
        {
            // Ensure session is enabled and configured in Program.cs / Startup.cs

            var errorObjectInSession = HttpContext.Session.GetObject<ErrorObjectViewModel>(_constants.ErrorSessionObjectName);

            if (errorObjectInSession != null)
            {
                errorLogId = errorObjectInSession.ErrorLogId ?? "";
                messages = errorObjectInSession.Exception?.ToString() ?? "";
                HttpContext.Session.Remove(_constants.ErrorSessionObjectName);
            }

            ViewBag.ErrorLogId = errorLogId;
            ViewBag.Messages = string.Equals(_constants.ShowErrorMessage, bool.TrueString, StringComparison.InvariantCultureIgnoreCase)
                ? messages
                : string.Empty;

            return View();
        }


        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult NoSurvey()
        {
            return View();
        }
    }
}