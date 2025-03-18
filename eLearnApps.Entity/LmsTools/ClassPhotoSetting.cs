using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class ClassPhotoSetting : BaseEntity
    {
        public int ClassPhotoSettingId { get; set; }
        public int CourseId { get; set; }

        public int PhotoSize { get; set; }

        public int PhotoPosition { get; set; }

        public bool IsPhotoDisplayed { get; set; }

        public bool IsFullNameDisplayed { get; set; }

        public bool IsUserNameDisplayed { get; set; }

        public bool IsNricFinDisplayed { get; set; }

        public bool IsSchoolDisplayed { get; set; }

        public int PageOrientation { get; set; }

        public int GroupBy { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}
