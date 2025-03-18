using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Business.Interface
{
    public interface ICourseTemplateService
    {
        CourseTemplate GetById(int id);
        void Insert(CourseTemplate courseTemplate);
        void Update(CourseTemplate courseTemplate);
        void Delete(CourseTemplate courseTemplate);
        void Insert(List<CourseTemplate> courseTemplates);
        void Save(List<CourseTemplate> courseTemplates);
    }
}