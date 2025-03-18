using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.Common;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.RPT
{
    public class GradeBookViewModel
    {
        public List<GradeObjectViewModel> GradeObjects { get; set; }
        public List<UserGradeObjectViewModel> UserGradeObjects { get; set; }
        public List<UserViewModel> Students { get; set; }
        public List<UserViewModel> Instructors { get; set; }
        public List<SectionGradeBook> SectionGradeBooks { get; set; }
        public List<UserViewModel> StudentWithInvalidMark { get; set; }
        public bool IsSubmitted { get; set; }
        public bool MixedGradeType { get; set; }

        public GradeBookViewModel()
        {
            GradeObjects = new List<GradeObjectViewModel>();
            UserGradeObjects = new List<UserGradeObjectViewModel>();
            SectionGradeBooks = new List<SectionGradeBook>();
            MixedGradeType = false;
        }
    }

    public class SectionGradeBook
    {
        public SectionInfo SectionInfo { get; set; }
        public List<UserGradeObjectViewModel> UserGradeObjects { get; set; }
    }

    public class SectionInfo
    {
        public bool IsSubmitted { get; set; }
        public int SectionId { get; set; }
        public List<UserDto> Students { get; set; }
        public List<UserDto> Instructors { get; set; }
    }
}
