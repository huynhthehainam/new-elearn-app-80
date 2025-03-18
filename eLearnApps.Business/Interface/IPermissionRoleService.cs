using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Business.Interface
{
    public interface IPermissionRoleService
    {
        void Insert(PermissionRole permissionRole);
        void Delete(PermissionRole permissionRole);
        void Insert(List<PermissionRole> permissionRoles);
        void Delete(List<PermissionRole> permissionRoles);
        List<PermissionRole> GetPermissionRoles();
        void Update(PermissionRole permissionRole);
        void Update(List<PermissionRole> permissionRoles);
        PermissionRole FindByPermissionRole(PermissionRole permissionRole);
        List<PermissionRole> FindByRoleId(List<int?> rolesId);
        List<PermissionRole> FindByRoleId(int roleId);
        List<PermissionRole> FindByPermissionIdWithRoleIds(int permissionId, List<int?> lstRoleId);
        void ResetPermissionRole();
    }
}