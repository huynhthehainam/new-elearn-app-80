using eLearnApps.Business.Interface;
using eLearnApps.Core.Caching;
using eLearnApps.CustomAttribute;
using eLearnApps.Entity.Security;
using eLearnApps.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace eLearnApps.Controllers
{
    [Authorize, TrackingLog]
    public class TestController : BaseController
    {
        private readonly IIcsService _icsService;
        private readonly ICmtService _cmtService;
        private readonly IPeerFeedbackService _peerFeedbackService;
        public TestController(IIcsService icsService, ICmtService cmtService, ICacheManager cacheManager, IErrorLogService errorLogService, IPeerFeedbackService peerFeedbackService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IServiceProvider serviewProvider, ICompositeViewEngine viewEngine, IViewRenderService viewRenderService) : base(cacheManager, errorLogService, httpContextAccessor, configuration, serviewProvider, viewEngine)
        {
            _icsService = icsService;
            _cmtService = cmtService;
            _peerFeedbackService = peerFeedbackService;
        }

        [HttpGet]
        public IActionResult Index() => Content("Routing works!asfasfasf");
    }
}
