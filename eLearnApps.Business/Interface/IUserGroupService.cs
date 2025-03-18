using eLearnApps.Entity.LmsTools;
using System.Collections.Generic;

namespace eLearnApps.Business.Interface
{
    public interface IUserGroupService
    {
        UserGroup GetById(int id);
        List<UserGroup> GetByCategoryId(int categoryGroupId);
        List<int> GetUserIdByCategoryIds(List<int> categoryGroupIds);
        void Insert(UserGroup user);
        void Update(UserGroup user);
        void Delete(UserGroup user);
        void Insert(List<UserGroup> users);
        void Delete();
        void Save(List<UserGroup> userGroups);
    }
}
