using eLearnApps.Entity.LmsTools;
using System.Collections.Generic;

namespace eLearnApps.Business.Interface
{
    public interface ICourseCategoryService
    {
        CourseCategory GetById(int id);
        void Insert(CourseCategory courseCategory);
        void Update(CourseCategory courseCategory);
        void Delete(CourseCategory courseCategory);
        void Insert(List<CourseCategory> courseCategories);
        void Delete();
        List<Entity.LmsTools.Dto.CourseCategory> GetByCourseId(int courseId);
        void Save(List<CourseCategory> courseCategories);
        List<CourseCategory> GetByListId(List<int> listIds);
        List<Entity.LmsTools.Dto.CourseCategoryDto> GetByCourseCategoryListId(List<int> ids);
    }
}
