using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class GroupCategoryData
    {
        public int GroupCategoryId { get; set; }
        public string EnrollmentStyle { get; set; }
        public int EnrollmentQuantity { get; set; }
        public bool AutoEnroll { get; set; }
        public bool RandomizeEnrollments { get; set; }
        public string Name { get; set; }
        public RichTextInput Description { get; set; }
        public int? MaxUsersPerGroup { get; set; }
    }
}
