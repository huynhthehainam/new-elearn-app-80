using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class GroupData
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public RichTextInput Description { get; set; }
        public List<int> Enrollments { get; set; }
    }
}
