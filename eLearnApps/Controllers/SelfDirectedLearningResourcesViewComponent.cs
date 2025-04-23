using eLearnApps.Entity.LmsTools;
using eLearnApps.ViewModel.PeerFeedback;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eLearnApps.Controllers
{
    public class SelfDirectedLearningResourcesViewComponent : ViewComponent
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Constants _constants;
        private readonly IWebHostEnvironment _env;
        public SelfDirectedLearningResourcesViewComponent(
            IWebHostEnvironment env,
            IConfiguration configuration,
            ILogger<SelfDirectedLearningResourcesViewComponent> log)
        {
            _constants = new Constants(configuration);
            _env = env;
        }
        public async Task<IViewComponentResult> InvokeAsync(string resourceId, string questionTitle)
        {
            _log.Info("**************** START SelfDirectedLearningResources ****************");

            var model = new SelfDirectedLearningResourcesModel();
            //var templateFile = $"{Server.MapPath(_constants.StaticFilesFolder)}/SelfDirectedLearningResources.json";
            var templateFile = Path.Combine(_env.WebRootPath, _constants.StaticFilesFolder, "SelfDirectedLearningResources.json");
            var content = System.IO.File.ReadAllText(templateFile);
            if (!string.IsNullOrEmpty(content))
            {
                _log.Info("json convert to List<SelfDirectedLearningResourcesModel>");
                var items = JsonSerializer.Deserialize<List<SelfDirectedLearningResourcesModel>>(content);
                if (string.Compare(questionTitle, _constants.ResponsibilityQuestionText, true) == 0)
                {
                    model = items.First(x => x.ItemType == 1);
                }
                else if (string.Compare(questionTitle, _constants.MeetsExpectationsQuestionText, true) == 0)
                {
                    model = items.First(x => x.ItemType == 2);
                }
                else if (string.Compare(questionTitle, _constants.ExceedsExpectationsQuestionText, true) == 0)
                {
                    model = items.First(x => x.ItemType == 3);
                }
                model.QuestionName = questionTitle;
                model.ResourceId = resourceId;
            }
            _log.Info("**************** END SelfDirectedLearningResources ****************");
            return View("_SelfDirectedLearningResources", model);
        }
    }
}
