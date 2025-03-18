using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps.Business
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IDbContext _dbContext;

        public UserGroupService(IDbContext dbContext, IServiceProvider serviceProvider)
        {
            _userGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserGroup>>("default");
            _dbContext = dbContext;
        }

        public List<UserGroup> GetByCategoryId(int categoryGroupId) => _userGroupRepository.Table.Where(x => x.CategoryGroupId == categoryGroupId).ToList();
        public List<int> GetUserIdByCategoryIds(List<int> categoryGroupIds) => _userGroupRepository.TableNoTracking
            .Where(x => categoryGroupIds.Contains(x.CategoryGroupId.Value))
            .Select(x => x.UserId.Value).ToList();
        public void Delete(UserGroup userGroup)
        {
            _userGroupRepository.Delete(userGroup);
        }

        public UserGroup GetById(int id)
        {
            return _userGroupRepository.GetById(id);
        }

        public void Insert(List<UserGroup> userGroups)
        {
            _userGroupRepository.Insert(userGroups);
        }

        public void Delete()
        {
            _dbContext.ExecuteSqlCommand("DELETE FROM UserGroups", true);
        }

        public void Insert(UserGroup userGroup)
        {
            _userGroupRepository.Insert(userGroup);
        }

        public void Update(UserGroup userGroup)
        {
            _userGroupRepository.Update(userGroup);
        }

        // this save function is not part of data layer. need to move it out to business layer
        // todo: delete deleted groups
        public void Save(List<UserGroup> userGroups)
        {
            var categoryGroupIds = userGroups.Where(u => u.CategoryGroupId.HasValue).Select(u => u.CategoryGroupId.Value).ToList();
            var userGroupsInDb = (from usergroup in _userGroupRepository.Table
                                  where categoryGroupIds.Contains(usergroup.CategoryGroupId.Value)
                                  select usergroup).ToList();

            var itemsToCompare = (from d2lUserGroup in userGroups
                                  join dbUG in userGroupsInDb on
                                         new { key1 = d2lUserGroup.CategoryGroupId, key2 = d2lUserGroup.UserId }
                                         equals new { key1 = dbUG.CategoryGroupId, key2 = dbUG.UserId }
                                         into dbUserGroups
                                  from dbUserGroup in dbUserGroups.DefaultIfEmpty()
                                  select new { incoming = d2lUserGroup, existing = dbUserGroup }).ToList();

            // this table is for many to many relation between users and groups, so there won't be any update
            var newItem = itemsToCompare
                .Where(item => item.existing == null)
                .Select(item => item.incoming).ToList();

            if (newItem.Count > 0) _userGroupRepository.Insert(newItem);

            var userGroupToDelete =
                from dbUserGroup in userGroupsInDb
                join d2lUG in userGroups on
                        new { key1 = dbUserGroup.CategoryGroupId, key2 = dbUserGroup.UserId }
                        equals new { key1 = d2lUG.CategoryGroupId, key2 = d2lUG.UserId }
                        into d2lUserGroups
                from d2lUserGroup in d2lUserGroups.DefaultIfEmpty()
                where d2lUserGroup == null
                select dbUserGroup;
            _userGroupRepository.Delete(userGroupToDelete);
        }
    }
}