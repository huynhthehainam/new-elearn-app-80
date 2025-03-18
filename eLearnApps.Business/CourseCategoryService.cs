using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps.Business
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly IRepository<CourseCategory> _courseCategoryRepository;
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Course> _courseRepository;

        public CourseCategoryService(IDbContext context,
            IServiceProvider serviceProvider
            )
        {
            _categoryGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<CategoryGroup>>("default");
            _courseCategoryRepository = serviceProvider.GetRequiredKeyedService<IRepository<CourseCategory>>("default");
            _userGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserGroup>>("default");
            _dbContext = context;
            _courseRepository = serviceProvider.GetRequiredKeyedService<IRepository<Course>>("default"); ;
        }

        public CourseCategory GetById(int id)
        {
            return _courseCategoryRepository.GetById(id);
        }

        public void Insert(List<CourseCategory> courseCategories)
        {

            _courseCategoryRepository.Insert(courseCategories);
        }

        public void Delete()
        {
            _dbContext.ExecuteSqlCommand("DELETE FROM CourseCategories", true);
        }

        public void Insert(CourseCategory courseCategory)
        {
            _courseCategoryRepository.Insert(courseCategory);
        }

        public void Update(CourseCategory courseCategory)
        {
            _courseCategoryRepository.Update(courseCategory);
        }

        public void Delete(CourseCategory courseCategory)
        {
            _courseCategoryRepository.Delete(courseCategory);
        }

        public List<Entity.LmsTools.Dto.CourseCategory> GetByCourseId(int courseId)
        {
            var categoryGroupRepository = new Repository<CategoryGroup>(_dbContext);
            var courseCategories = _courseCategoryRepository.Table;
            var categoryGroups = categoryGroupRepository.Table;

            var query = courseCategories
                .Join(categoryGroups,
                    m => m.Id,
                    v => v.CourseCategoryId.Value,
                    (m, v) => new { m, v }).Where(x => x.m.CourseId == courseId)
                .Select(x => new Entity.LmsTools.Dto.CourseCategory
                {
                    Id = x.m.Id,
                    Name = x.m.Name,
                    CategoryGroupId = x.v.Id,
                    CategoryGroupName = x.v.Name
                });
            return query.ToList();
        }

        public void Save(List<CourseCategory> courseCategories)
        {
            var courseIds = courseCategories.Select(e => e.CourseId.Value).Distinct().ToList();
            var courseCategoryInDb = _courseCategoryRepository.TableNoTracking
                .Where(courseCategory => courseIds.Contains(courseCategory.CourseId.Value))
                .Select(courseCategory => courseCategory)
                .ToList();

            var itemsToCompare = (from d2lCourseCategory in courseCategories
                                  join dbCourseCategory in courseCategoryInDb on d2lCourseCategory.Id equals dbCourseCategory.Id into joinedCourseCategories
                                  from dbCC in joinedCourseCategories.DefaultIfEmpty()
                                  select new { d2lData = d2lCourseCategory, dbData = dbCC }
                                 ).ToList();
            var newItem = new List<CourseCategory>();
            var itemToUpdate = new List<CourseCategory>();
            foreach (var item in itemsToCompare)
            {
                if (item.dbData == null) newItem.Add(item.d2lData);
                else
                {
                    if (item.dbData.EnrollmentStyle != item.d2lData.EnrollmentStyle || item.dbData.EnrollmentQuantity != item.d2lData.EnrollmentQuantity ||
                        item.dbData.AutoEnroll != item.d2lData.AutoEnroll || item.dbData.RandomizeEnrollments != item.d2lData.RandomizeEnrollments ||
                        item.dbData.Name != item.d2lData.Name || item.dbData.TextDescription != item.d2lData.TextDescription ||
                        item.dbData.HtmlDescription != item.d2lData.HtmlDescription || item.dbData.MaxUsersPerGroup != item.d2lData.MaxUsersPerGroup ||
                        item.dbData.CourseId != item.d2lData.CourseId)
                    {
                        var toupdate = item.dbData;
                        toupdate.EnrollmentStyle = item.d2lData.EnrollmentStyle;
                        toupdate.EnrollmentQuantity = item.d2lData.EnrollmentQuantity;
                        toupdate.AutoEnroll = item.d2lData.AutoEnroll;
                        toupdate.RandomizeEnrollments = item.d2lData.RandomizeEnrollments;
                        toupdate.Name = item.d2lData.Name;
                        toupdate.TextDescription = item.d2lData.TextDescription;
                        toupdate.HtmlDescription = item.d2lData.HtmlDescription;
                        toupdate.MaxUsersPerGroup = item.d2lData.MaxUsersPerGroup;
                        toupdate.CourseId = item.d2lData.CourseId;
                        itemToUpdate.Add(toupdate);
                    }
                }
            }
            if (newItem.Count > 0) _courseCategoryRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _courseCategoryRepository.Update(itemToUpdate);

            // get course category to delete
            var courseCategoriesToDelete = (from dbCourseCategory in courseCategoryInDb
                                            join d2lCourseCategory in courseCategories on dbCourseCategory.Id equals d2lCourseCategory.Id into joinedCourseCategories
                                            from d2lCC in joinedCourseCategories.DefaultIfEmpty()
                                            where d2lCC == null
                                            select dbCourseCategory
                                ).ToList();

            // get category group to delete
            var courseCategoryIds = courseCategoriesToDelete.Select(c => c.Id).ToList();
            var categoryGroupToDelete = (from categoryGroup in _categoryGroupRepository.TableNoTracking
                                         where courseCategoryIds.Contains(categoryGroup.CourseCategoryId.Value)
                                         select categoryGroup).ToList();
            // get user group to delete
            var categoryGroupIds = categoryGroupToDelete.Select(c => c.Id).ToList();
            var usergroupToDelete = (from usergroup in _userGroupRepository.TableNoTracking
                                     where categoryGroupIds.Contains(usergroup.CategoryGroupId.Value)
                                     select usergroup).ToList();

            _userGroupRepository.Delete(usergroupToDelete);
            _categoryGroupRepository.Delete(categoryGroupToDelete);
            _courseCategoryRepository.Delete(courseCategoriesToDelete);
        }

        public List<CourseCategory> GetByListId(List<int> listIds)
        {
            var query = _courseCategoryRepository.TableNoTracking.Where(x => listIds.Contains(x.Id)).ToList();
            return query;
        }
        public List<Entity.LmsTools.Dto.CourseCategoryDto> GetByCourseCategoryListId(List<int> ids)
        {
            var query = from course in _courseRepository.TableNoTracking
                        join courseCategory in _courseCategoryRepository.TableNoTracking on course.Id equals courseCategory.CourseId
                        where ids.Contains(courseCategory.Id)
                        select new Entity.LmsTools.Dto.CourseCategoryDto
                        {
                            CourseCategoryId = courseCategory.Id,
                            CourseId = course.Id,
                            CourseName = course.Name,
                        };
            return query.ToList();
        }
    }
}