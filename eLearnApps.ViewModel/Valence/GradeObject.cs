using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class GradeObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string GradeType { get; set; }
        public int CategoryId { get; set; }
        public RichTextInput Description { get; set; }
        public int Weight { get; set; }
        public string ActivityId { get; set; }
        public int MaxPoints { get; set; }
        public bool CanExceedMaxPoints { get; set; }
        public bool IsBonus { get; set; }
        public bool ExcludeFromFinalGradeCalculation { get; set; }
        public int? GradeSchemeId { get; set; }
        public string GradeSchemeUrl { get; set; }
    }
}
