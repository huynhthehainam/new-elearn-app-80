using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class GradeObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string GradeType { get; set; }
        public decimal Weight { get; set; }
        public decimal MaxPoints { get; set; }
        public int? GradeSchemeId { get; set; }
        public string GradeSchemeUrl { get; set; }


        /// <summary>
        /// TODO: deprecated
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// TODO: deprecated
        /// </summary>
        public RichTextInput Description { get; set; }
        /// <summary>
        /// TODO: deprecated
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// TODO: deprecated
        /// </summary>
        public bool CanExceedMaxPoints { get; set; }
        /// <summary>
        /// TODO: deprecated
        /// </summary>
        public bool IsBonus { get; set; }
        /// <summary>
        /// TODO: deprecated
        /// </summary>
        public bool ExcludeFromFinalGradeCalculation { get; set; }
    }
}
