using eLearnApps.Business.Interface;
using eLearnApps.Core.Caching;
using eLearnApps.CustomAttribute;
using eLearnApps.ViewModel.KendoUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace eLearnApps.Controllers
{
    [Authorize, TrackingLog]
    public class MenuController : BaseController
    {
        private readonly ISemesterService _semesterService;
        private readonly ICourseService _courseService;
        public MenuController(
        ISemesterService semesterService,
        ICourseService courseService,
        ICacheManager cacheManager,
          IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
             IServiceProvider serviewProvider,
            ICompositeViewEngine compositeViewEngine,
            IErrorLogService errorLogService) : base(cacheManager, errorLogService, httpContextAccessor, configuration, serviewProvider, compositeViewEngine)
        {
            _courseService = courseService;
            _semesterService = semesterService;
        }
        public PartialViewResult _LeftMenuEE()
        {
            return PartialView(MenuName);
        }
        public PartialViewResult _LeftMenu()
        {
            var temp = ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.ToList();

            return PartialView(MenuName);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> PopulateDropdown(int? semesterId)
        {
            var semesters = await _semesterService.GetAllByUserIdAsync(UserInfo.UserId);
            var result = semesters.Select(x => new TreeViewItem
            {
                id = x.Id,
                text = x.Name,
                hasChildren = true,
                expanded = x.Id == semesterId.Value,
            }).OrderByDescending(s => s.id).ToList();
            return Json(result);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> GetBySemesterId(int? semesterId)
        {
            var lstCourse = new List<Entity.LmsTools.Course>();
            if (semesterId.HasValue)
            {
                lstCourse = await _courseService.GetBySemesterIdAsync(UserInfo.UserId, semesterId.Value);
            }
            return Json(lstCourse.Select(x => new NodeItem
            {
                id = x.Id,
                text = x.Name
            }).OrderByDescending(item => item.id).ToList());
        }
    }

}
