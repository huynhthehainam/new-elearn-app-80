using Microsoft.AspNetCore.Mvc.Rendering;

namespace eLearnApps.ViewModel.FFTS
{
    public class LogListViewModel
    {
        public LogListViewModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
        public string Message { get; set; }
        public int LogLevel { get; set; }
        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}