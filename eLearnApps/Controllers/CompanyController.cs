using Microsoft.AspNetCore.Mvc;

namespace eLearnApps.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { message = "Hello from CompanyController" });
        }
    }
}
