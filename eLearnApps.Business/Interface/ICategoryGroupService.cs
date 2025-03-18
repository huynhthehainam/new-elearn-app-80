using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Business.Interface
{
    public interface ICategoryGroupService
    {
        /// <summary>
        /// get category group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CategoryGroup GetById(int id);
        void Insert(CategoryGroup categoryGRoup);
        void Update(CategoryGroup categoryGroup);
        void Delete(CategoryGroup categoryGroup);
        void Insert(List<CategoryGroup> categoryGroups);
        void Delete();
        /// <summary>
        /// Get category group by course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<CategoryGroup> GetByCourseId(int courseId);
        /// <summary>
        ///  Get category with group by course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="listNormal"></param>
        /// <param name="listMerge"></param>
        /// <returns></returns>
        void GetCategoryWithGroup(int courseId, ref List<CategorySectionGroup> listNormal, ref List<CategorySectionGroup> listMerge);
        /// <summary>
        /// Get category group by ids
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        List<DefaultEntity> GetListByGroupIds(int courseId, List<int> groupIds);
        /// <summary>
        /// Get data group section for my evaluation
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="evaluationSessionId"></param>
        /// <returns></returns>
        List<DefaultEntity> GetMyEvaluationResponseGroup(int courseId, int evaluationSessionId);
        /// <summary>
        /// Get data group section for my evaluation
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="evaluationSessionId"></param>
        /// <returns></returns>
        List<MyEvaluationResponseGroupDto> GetMyEvaluationResponseForGroup(int courseId, int evaluationSessionId);
        /// <summary>
        /// Get category group by course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<CategoryGroupUser> GetListByCourse(int courseId);
        /// <summary>
        /// Get category group by course and role
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        List<CategoryGroupUser> GetListByCourseWithRole(int courseId, List<int> roles);
        void Save(List<CategoryGroup> categoryGroups);
        List<CategorySectionGroup> GetCategoryWithGroupByCourseMerge(int courseId);
        List<int> GetCategoryIdsByCourseId(List<int?> courseIds);
        List<CategoryGroup> GetListByGroupIds(List<int> groupIds);
    }
}