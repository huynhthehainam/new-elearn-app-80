using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface ICategoryGroupDao
    {
        /// <summary>
        /// Get category group by list merge course
        /// </summary>
        /// <param name="listCourse"></param>
        /// <returns></returns>
        List<CategorySectionGroup> GetListByMergeCourse(List<int> listCourse);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<CategorySectionGroup>> GetListGroupByCourseCategoryId(int courseId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseOfferingCodes"></param>
        /// <returns></returns>
        Task<List<UserCategoryGroupDto>> GetUserCategoryGroupAsync(List<string> courseOfferingCodes);
    }
}