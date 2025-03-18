using eLearnApps.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class GradeObjectViewModel
    {
        // todo need to find a way to represent Rank, Final and rounded final
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string HtmlDisplayName {
            get
            {
                if (string.IsNullOrWhiteSpace(CategoryName)) return Name;
                else
                {
                    return $"<b>{CategoryName}</b><br />{Name}";
                }
            }
        }
        public string PlainTextDisplayName { get; set; }
        public string GradeType { get; set; }
        public decimal? Weight { get; set; }
        public decimal MaxPoints { get; set; }

        public int GradeSchemeId { get; set; }
        public string GradeSchemeUrl { get; set; }

        /// <summary>
        /// Useful when constructing kendo grid column
        /// </summary>
        public string KendoGridColumnId 
        {
            get
            {
                switch (Id)
                {
                    case (int)GradeObjectType.Final:
                        return "GradeObject_Final";
                    case (int)GradeObjectType.Rank:
                        return "GradeObject_Rank";
                    default:
                        return $"GradeObject_{Id}";
                }
            }
        }

        /// <summary>
        /// Useful when constructing kendo grid with 2 levels
        /// </summary>
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal? CategoryWeight { get; set; }
        public decimal? CategoryMaxPoints { get; set; }
        public string CategoryKendoGridColumnId => CategoryId > 0 ? $"GradeObject_{CategoryId}" : string.Empty;
    }
}
