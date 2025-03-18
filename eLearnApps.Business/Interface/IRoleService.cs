using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Business.Interface
{
    public interface IRoleService
    {
        Role GetById(int id);
        void Insert(Role role);
        void Update(Role role);
        void Delete(Role role);
        void Insert(List<Role> roles);
        void Save(List<Role> role);
        List<Role> GetRoles();
        List<Role> FindByIds(List<int?> ids);

        /// <summary>
        /// Get Role by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Role> GetRoleByUserId(int userId);
    }
}