using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class SectionData
    {
        public int SectionId { get; set; }
        public string? Name { get; set; }
        public RichTextInput? Description { get; set; }
        public List<int> Enrollments { get; set; }
        public List<Entity.Valence.OrgUnitUser> Classlist { get; set; }
    }
}
