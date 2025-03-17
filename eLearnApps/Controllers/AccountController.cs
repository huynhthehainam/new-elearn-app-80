using eLearnApps.Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLearnApps.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAppSettingService _appSettingService;
        private readonly IConfiguration _configuration;
        public AccountController(IAppSettingService appSettingService, IConfiguration configuration)
        {
            _appSettingService = appSettingService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        public IActionResult LtiView()
        {
            var showLoginPage = _appSettingService.GetByKey("ShowLoginPage");
            if (showLoginPage.Value.ToLower() == "true")
            {
                return View("Lti");
            }
            else
            {
                var domain = _configuration.GetValue<string>("Domain");
                return Redirect($"https://{domain}");
            }
        }
    }
}
