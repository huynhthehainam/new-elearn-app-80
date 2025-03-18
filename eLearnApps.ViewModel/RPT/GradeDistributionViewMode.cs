using eLearnApps.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
  public  class GradeDistributionViewModel
    {
        public GradeDistributionViewModel()
        {
            StudentCountModels = new List<StudentCountModel>();
            BarChartModels = new List<BarChartModel>();
            GradeDistributionModels = new List<GradeDistributionModel>();
            GradeStatisticModels =  new  List<GradeStatisticModel>();
            Siblings = new List<CourseViewModel>();
        }

        public List<StudentCountModel> StudentCountModels { set; get; }
        public List<BarChartModel>  BarChartModels { set; get; }
        public List<GradeDistributionModel> GradeDistributionModels { set; get; }
        public List<GradeStatisticModel> GradeStatisticModels { set; get; }
        public List<CourseViewModel> Siblings { set; get; }
        public string CourseDisplayName { get; set; }
        public string Instructors { get; set; }
    }
}
