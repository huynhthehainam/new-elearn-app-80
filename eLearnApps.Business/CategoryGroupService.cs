#region USING

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Resources;
using System.Xml.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Business.Resources;
using eLearnApps.Core;
using eLearnApps.Core.Caching;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using Microsoft.Extensions.DependencyInjection;
using CourseCategory = eLearnApps.Entity.LmsTools.CourseCategory;

#endregion

namespace eLearnApps.Business
{
    public class CategoryGroupService : BaseService, ICategoryGroupService
    {
        #region REPOSITORY
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<CourseCategory> _courseCategoryRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly ICategoryGroupDao _categoryGroupDao;
        private readonly ICacheManager _cacheManager;
        private readonly ICourseService _courseService;
        #endregion

        public CategoryGroupService(IDbContext context,
          IServiceProvider serviceProvider,
            ICacheManager cacheManager,
            IDaoFactory factory,
            ICourseService courseService)
        {
            _categoryGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<CategoryGroup>>("default");
            _courseCategoryRepository = serviceProvider.GetRequiredKeyedService<IRepository<CourseCategory>>("default");
            _userGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserGroup>>("default");
            _dbContext = context;
            _cacheManager = cacheManager;
            _courseService = courseService;


            _categoryGroupDao = factory.CategoryGroupDao;
        }
        /// <summary>
        /// Get category group by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CategoryGroup GetById(int id)
        {
            return _categoryGroupRepository.GetById(id);
        }

        /// <summary>
        /// insert category
        /// </summary>
        /// <param name="categoryGroups"></param>
        public void Insert(List<CategoryGroup> categoryGroups)
        {
            _categoryGroupRepository.Insert(categoryGroups);
        }
        /// <summary>
        /// Delete category
        /// </summary>
        public void Delete()
        {
            _dbContext.ExecuteSqlCommand("DELETE FROM CategoryGroups", true);
        }
        /// <summary>
        /// Get category group by course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<CategoryGroup> GetByCourseId(int courseId)
        {
            var queryCategoryGroup = _categoryGroupRepository.Table;
            var queryCourseCategory = _courseCategoryRepository.Table;
            var result = queryCategoryGroup.Join(queryCourseCategory, category => category.CourseCategoryId,
                courseCategory => courseCategory.Id, (category, courseCategory) => new CategoryGroup
                {
                    Id = category.Id,
                    CourseCategoryId = category.CourseCategoryId,
                    HtmlDescription = category.HtmlDescription,
                    Name = category.Name,
                    TextDescription = category.TextDescription
                });
            var categoryCourse = result.ToList();
            return categoryCourse;
        }
        public List<int> GetCategoryIdsByCourseId(List<int?> courseIds)
        {
            var queryCategoryGroup = _categoryGroupRepository.Table;
            var queryCourseCategory = _courseCategoryRepository.Table.Where(x => courseIds.Contains(x.CourseId));
            var query = from category in queryCategoryGroup
                        join courseCategory in queryCourseCategory on category.CourseCategoryId equals courseCategory.Id
                        select category.Id;
            return query.ToList();
        }
        /// <summary>
        /// Get category with group by course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="listNormal"></param>
        /// <param name="listMerge"></param>
        /// <returns></returns>
        public void GetCategoryWithGroup(int courseId, ref List<CategorySectionGroup> listNormal, ref List<CategorySectionGroup> listMerge)
        {
            var queryCategoryGroup = _categoryGroupRepository.Table;
            var queryCourseCategory = _courseCategoryRepository.Table;
            var queryUserGroup = _userGroupRepository.Table;
            try
            {
                var resultCourseCategory = queryCourseCategory.Where(x => x.CourseId == courseId).Select(x =>
                    new CategorySectionGroup
                    {
                        CategoryId = null,
                        CourseCategoryId = x.Id,
                        Name = null,
                        CourseCategoryName = x.Name
                    });
                var result = from cg in queryCategoryGroup
                             join cc in queryCourseCategory on cg.CourseCategoryId equals cc.Id
                             join ug in queryUserGroup on cg.Id equals ug.CategoryGroupId into categoryWithGroup
                             from usg in categoryWithGroup.DefaultIfEmpty()
                             where cc.CourseId == courseId
                             select new CategorySectionGroup
                             {
                                 CategoryId = cg.Id,
                                 CourseCategoryId = cc.Id,
                                 Name = cg.Name,
                                 CourseCategoryName = cc.Name
                             };
                listNormal = resultCourseCategory.Union(result).ToList();

                listMerge = GetCategoryWithGroupByCourseMerge(courseId);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// Get list category group by ids
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        public List<DefaultEntity> GetListByGroupIds(int courseId, List<int> groupIds)
        {
            string param = string.Join(",", groupIds);
            return _dbContext.SqlQuery<DefaultEntity>(string.Format(Query.CategoryGroup_GetListByGroupIds, courseId, param)).ToList();

        }
        /// <summary>
        /// Get data group, section for my evaluation
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="evaluationSessionId"></param>
        /// <returns></returns>
        public List<DefaultEntity> GetMyEvaluationResponseGroup(int courseId, int evaluationSessionId)
        {
            return _dbContext.SqlQuery<DefaultEntity>(string.Format(Query.CategoryGroup_GetMyEvaluationResponseGroup, courseId, evaluationSessionId)).ToList();
        }
        /// <summary>
        /// Get data group, section for my evaluation
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="evaluationSessionId"></param>
        /// <returns></returns>
        public List<MyEvaluationResponseGroupDto> GetMyEvaluationResponseForGroup(int courseId, int evaluationSessionId)
        {
            return _dbContext.SqlQuery<MyEvaluationResponseGroupDto>(string.Format(Query.CategoryGroup_GetMyEvaluationResponseGroup, courseId, evaluationSessionId)).ToList();
        }
        /// <summary>
        /// Get category group by course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<CategoryGroupUser> GetListByCourse(int courseId)
        {
            var result = _dbContext.SqlQuery<CategoryGroupUser>(string.Format(Query.CategoryGroup_GetListByCourse, courseId)).ToList();
            return result;
        }
        /// <summary>
        /// Get category group by course and role
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public List<CategoryGroupUser> GetListByCourseWithRole(int courseId, List<int> roles)
        {
            var result = _dbContext.SqlQuery<CategoryGroupUser>(string.Format(Query.CategoryGroup_GetListByCourseWithRole, courseId, string.Join(",", roles))).ToList();
            return result;
        }

        public void Insert(CategoryGroup categoryGroup)
        {
            _categoryGroupRepository.Insert(categoryGroup);
        }

        public void Update(CategoryGroup categoryGroup)
        {
            _categoryGroupRepository.Update(categoryGroup);
        }

        public void Delete(CategoryGroup categoryGroup)
        {
            _categoryGroupRepository.Delete(categoryGroup);
        }

        public void Save(List<CategoryGroup> categoryGroups)
        {
            // these are all courseCategory ids under selected courses
            var courseCategoryIds = categoryGroups.Where(cg => cg.CourseCategoryId.HasValue).Select(cg => cg.CourseCategoryId.Value).ToList();
            var categoryGroupsInDb = (from categoryGroup in _categoryGroupRepository.TableNoTracking
                                      where courseCategoryIds.Contains(categoryGroup.CourseCategoryId.Value)
                                      select categoryGroup).ToList();

            var itemsToCompare = (from d2lCategoryGroup in categoryGroups
                                  join dbCG in categoryGroupsInDb on d2lCategoryGroup.Id equals dbCG.Id into dbCategoryGroups
                                  from dbCategoryGroup in dbCategoryGroups.DefaultIfEmpty()
                                  select new { d2lData = d2lCategoryGroup, dbData = dbCategoryGroup }).ToList();
            var newItem = new List<CategoryGroup>();
            var itemToUpdate = new List<CategoryGroup>();
            foreach (var item in itemsToCompare)
            {
                if (item.dbData == null) newItem.Add(item.d2lData);
                else
                {
                    if (item.dbData.Name != item.d2lData.Name || item.dbData.CourseCategoryId != item.d2lData.CourseCategoryId ||
                        item.dbData.TextDescription != item.d2lData.TextDescription || item.dbData.HtmlDescription != item.d2lData.HtmlDescription)
                    {
                        var toupdate = item.dbData;
                        toupdate.Name = item.d2lData.Name;
                        toupdate.CourseCategoryId = item.d2lData.CourseCategoryId;
                        toupdate.TextDescription = item.d2lData.TextDescription;
                        toupdate.HtmlDescription = item.d2lData.HtmlDescription;
                        itemToUpdate.Add(toupdate);
                    }
                }
            }

            if (newItem.Count > 0) _categoryGroupRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _categoryGroupRepository.Update(itemToUpdate);

            var categoryGroupsToDelete =
                from dbCategoryGroup in categoryGroupsInDb
                join dlCG in categoryGroups on dbCategoryGroup.Id equals dlCG.Id into d2lCategoryGroups
                from d2lCategoryGroup in d2lCategoryGroups.DefaultIfEmpty()
                where d2lCategoryGroup == null
                select dbCategoryGroup;

            var categoryGroupIds = categoryGroupsToDelete.Select(c => c.Id).ToList();
            var userGroupToDelete = from dbUserGroup in _userGroupRepository.Table
                                    where categoryGroupIds.Contains(dbUserGroup.CategoryGroupId.Value)
                                    select dbUserGroup;

            _categoryGroupRepository.Delete(categoryGroupsToDelete);
            _userGroupRepository.Delete(userGroupToDelete);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<CategorySectionGroup> GetCategoryWithGroupByCourseMerge(int courseId)
        {
            var cacheMergeCourseKey = $"{CacheDataKey.MergeCourse.ToString()}_{courseId}";
            var lstMergeCourse = _cacheManager.Get(cacheMergeCourseKey, () =>
            {
                var lstResult = _courseService.GetCourseSections(courseId).Where(x => x.IsMerge == false).ToList();
                if (lstResult.Count > 0)
                {
                    var res = lstResult.Select(x => new CategorySectionGroup
                    {
                        Name = x.SectionName,
                        CourseCategoryName = x.SectionName,
                        CategoryId = x.Id,
                        CourseId = x.Id
                    }).ToList();
                    return res;
                }
                return null;
            });
            return lstMergeCourse;
        }

        public List<CategoryGroup> GetListByGroupIds(List<int> groupIds)
        {
            return _categoryGroupRepository.TableNoTracking.Where(x => groupIds.Contains(x.Id)).ToList();
        }
    }
}