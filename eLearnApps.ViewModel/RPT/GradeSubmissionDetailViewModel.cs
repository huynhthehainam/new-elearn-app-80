using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class CourseSubmissionPageViewModel
    {
        public List<CourseSubmissionViewModel> Coursesubmissions { get; set; }
        public List<KendoUI.KendoGridGroupColumn> GradeObjects { get; set; }
        public List<Dictionary<string, string>> RecommendedTutors { get; set; }
        public List<Dictionary<string, string>> EligibleTutors { get; set; }
    }

    public class PeerTutoringTutorViewModel
    {
        public List<GradeResultModel> GradeResults { set; get; }
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string SectionName { get; set; }
        public string Email { get; set; }
        public string RecommendedBy { get; set; }
        public DateTime RecommendedOn { get; set; }
    }


    public class CourseSubmissionViewModel
    {
        public string CourseKey { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string SubmittedBy { get; set; }
        public string SubmissionDate { get; set; }
    }

    public class GradeSubmissionDetailViewModel
    {
        public string OrgDefinedId { set; get; }
        public string DisplayName { set; get; }
        public int Mark { set; get; }
        public string Grade { set; get; }
    }
}
