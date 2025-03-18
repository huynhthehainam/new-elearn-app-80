using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{

    public class GradeResultModel
    {
        public string GradeName { set; get; }
        public string GradeType { get; set; }
        public string Marks { set; get; }
        public string Grade { set; get; }
        public string Weight { set; get; }
        public string AverageMarks { set; get; }
        public string AverageGrade { set; get; }
        public string MinMarks { set; get; }
        public string MinGrade { set; get; }
        public string MaxMarks { set; get; }
        public string MaxGrade { set; get; }
        public GradeResultModel()
        {
            GradeName = string.Empty;
            GradeType = string.Empty;
            Marks = string.Empty;
            Grade = string.Empty;
            Weight = string.Empty;
            AverageGrade = string.Empty;
            AverageMarks = string.Empty;
            MinMarks = string.Empty;
            MinGrade = string.Empty;
            MaxMarks = string.Empty;
            MaxGrade = string.Empty;
        }

    }
}
