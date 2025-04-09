using System;
using System.Net;
using eLearnApps.Business;
using eLearnApps.Business.Interface;
using eLearnApps.Core.Caching;
using eLearnApps.CustomAttribute;
using eLearnApps.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace eLearnApps.Controllers
{
    [Authorize]
    [TrackingLog]
    public class CommonController : BaseController
    {
        private readonly ClaimHelper _claimHelper;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public CommonController(IWebHostEnvironment env, ICacheManager cacheManager, IErrorLogService errorLogService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IServiceProvider serviewProvider, ICompositeViewEngine viewEngine) : base(cacheManager, errorLogService, httpContextAccessor, configuration, serviewProvider, viewEngine)
        {
            _claimHelper = new ClaimHelper(serviewProvider);
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetUserGuideLink(string toolId)
        {
            var enrollment = _claimHelper.GetEnrollmentFromCache(UserInfo.UserId, CourseId);

            var isStudent = string.Equals(enrollment.RoleName, RoleName.Student.ToString(), StringComparison.InvariantCultureIgnoreCase);
            string path = string.Empty;
            toolId = string.IsNullOrWhiteSpace(toolId) ? "ffts" : toolId.ToLower();
            switch (toolId)
            {
                case "rpt":
                    if (isStudent)
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/student/userguides/MyResults-StudentUserGuide.pdf";
                    else
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/faculty_staff/userguides/ResultsProcessingTool-FacultyUserGuide.pdf";
                    break;

                case "cmt":
                    if (isStudent)
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/student/userguides/MyAttendance-StudentUserGuide.pdf";
                    else
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/faculty_staff/userguides/ClassManagementTool-FacultyUserGuide.pdf";
                    break;

                case "journal":
                    if (isStudent)
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/student/userguides/Journal-StudentUserGuide.pdf";
                    else
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/faculty_staff/userguides/Journal-FacultyUserGuide.pdf";
                    break;

                case "pet":
                    if (isStudent)
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/student/userguides/PeerEvaluationTool-StudentUserGuide.pdf";
                    else
                        path = "https://smu.sharepoint.com/sites/iits/elearnsupport/Documents/faculty_staff/userguides/PeerEvaluationTool-FacultyUserGuide.pdf";
                    break;
                case "ee":
                    var externalUrl = _configuration.GetValue<string>("UserGuideDownloadUrl");
                    if (string.IsNullOrEmpty(externalUrl))
                    {
                        var userGuideRelativePath = "guide/EE_Instructor_UserGuide.pdf";
                        var userGuideFullPath = Path.Combine(_env.WebRootPath, "Content", userGuideRelativePath);

                        if (System.IO.File.Exists(userGuideFullPath))
                        {
                            path = Url.Content($"~/Content/{userGuideRelativePath}");
                        }
                    }
                    else
                    {
                        path = externalUrl;
                    }
                    break;
                default:
                    path = GetDefaultPath(isStudent, toolId);
                    break;
            }

            return Json(path);
        }

        private string GetDefaultPath(bool isStudent, string prefix)
        {
            string path = string.Empty;

            var fileName = isStudent
                ? $"{prefix}_Student_UserGuide.pdf"
                : $"{prefix}_Instructor_UserGuide.pdf";

            var relativePath = Path.Combine("Content", "guide", fileName);
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
            {
                // Ensure path uses forward slashes
                path = Url.Content($"~/{relativePath.Replace("\\", "/")}");
            }

            return path;
        }
    }
}