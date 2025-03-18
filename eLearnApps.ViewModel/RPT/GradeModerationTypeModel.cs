using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class GradeModerationTypeModel
    {
        public int ModerationTypeId { get; set; }
        public string Name { get; set; }

        public GradeModerationTypeModel(int moderationTypeId, string name)
        {
            ModerationTypeId = moderationTypeId;
            Name = name;
        }
    }
}
