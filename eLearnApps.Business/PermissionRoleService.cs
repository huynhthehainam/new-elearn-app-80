using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps.Business
{
    public class PermissionRoleService : IPermissionRoleService
    {
        private readonly IRepository<PermissionRole> _permissionRoleRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<Role> _roleRepository;

        public PermissionRoleService(IServiceProvider serviceProvider)
        {
            _permissionRoleRepository = serviceProvider.GetRequiredKeyedService<IRepository<PermissionRole>>("default");
            _roleRepository = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");
            _permissionRepository = serviceProvider.GetRequiredKeyedService<IRepository<Permission>>("default");
        }

        public void Insert(PermissionRole permissionRole) => _permissionRoleRepository.Insert(permissionRole);
        public void Delete(PermissionRole permissionRole) => _permissionRoleRepository.Delete(permissionRole);
        public void Insert(List<PermissionRole> permissionRoles) => _permissionRoleRepository.Insert(permissionRoles);
        public void Delete(List<PermissionRole> permissionRoles) => _permissionRoleRepository.Delete(permissionRoles);
        public List<PermissionRole> GetPermissionRoles() => _permissionRoleRepository.Table.ToList();
        public void Update(PermissionRole permissionRole) => _permissionRoleRepository.Update(permissionRole);
        public void Update(List<PermissionRole> permissionRoles) => _permissionRoleRepository.Update(permissionRoles);
        public PermissionRole FindByPermissionRole(PermissionRole permissionRole) => _permissionRoleRepository.Table.FirstOrDefault(x => x.PermissionId == permissionRole.PermissionId && x.RoleId == permissionRole.RoleId);
        public List<PermissionRole> FindByRoleId(List<int?> rolesId) => rolesId == null ? null : _permissionRoleRepository.Table.Where(x => rolesId.Contains(x.RoleId)).ToList();
        public List<PermissionRole> FindByRoleId(int roleId) => _permissionRoleRepository.Table.Where(x => x.RoleId == roleId).ToList();
        public List<PermissionRole> FindByPermissionIdWithRoleIds(int permissionId, List<int?> lstRoleId) => _permissionRoleRepository.Table.Where(x => x.PermissionId == permissionId && lstRoleId.Contains(x.RoleId)).ToList();

        public void ResetPermissionRole()
        {
            var roles = _roleRepository.Table.ToList();
            var adminPermission = _permissionRepository.Table.Select(p => p.Id).ToList();
            var instructorPermissions = GetInstructorPermission().ToList();
            var studentPermissions = GetStudentPermission().ToList();

            var permissionToDelete = new List<PermissionRole>();
            var newPermission = new List<PermissionRole>();
            foreach (var role in roles)
            {
                var rolename = role.Name.ToLower();
                var existingPermission = _permissionRoleRepository.Table.Where(pr => pr.RoleId == role.Id).ToList();

                if (rolename.Contains("school admin") || rolename.Contains("content creator") || rolename.Contains("external auditor"))
                {
                    // don't do anything
                }
                else if (rolename.Contains("admin"))
                {
                    // admin... have this access but not "school admin"

                    // exist in existing, but not in new
                    permissionToDelete.AddRange(
                        existingPermission
                        .Where(ep => !adminPermission.Contains(ep.PermissionId))
                    );

                    // exist in new, but not in existing
                    newPermission.AddRange(
                        adminPermission
                        .Where(ep => !existingPermission.Any(existing => existing.PermissionId == ep))
                        .Select(pd => new PermissionRole() { PermissionId = pd, RoleId = role.Id })
                    );
                }
            }
            _permissionRoleRepository.Insert(newPermission);
            _permissionRoleRepository.Delete(permissionToDelete);
        }

        private IList<int> GetInstructorPermission()
        {
            var instructorPermissions = _permissionRepository.Table.ToList()
                 .Where(p =>
                    p.SystemName == "AccessFfts" || p.SystemName == "AccessFftsSearch" ||  // ffts
                    p.SystemName == "ManageIcs" || // ICS
                    p.SystemName == "AccessCmt" || p.SystemName == "AttendanceList" || p.SystemName == "ClassPhoto" || p.SystemName == "WeeklyAttendance" || p.SystemName == "LargeSessionView" || // CMT
                    p.SystemName == "ManageExtraction" || // Exam extraction
                    p.SystemName == "EvaluationEntries" || p.SystemName == "ManageEvaluation" || p.SystemName == "MarkLabel" || p.SystemName == "EvaluationEntries" || // Peer Evaluation
                    p.SystemName == "AccessRpt"
                    )
                 .Select(p => p.Id)
                 .ToList();

            return instructorPermissions;
        }

        private IList<int> GetStudentPermission()
        {
            var instructorPermissions = _permissionRepository.Table.ToList()
                 .Where(p =>
                 p.SystemName == "AccessFfts" || p.SystemName == "AccessFftsSearch" ||  // FFTS
                 p.SystemName == "FeedbackIcs" || // ICS
                 p.SystemName == "AccessCmt" || p.SystemName == "MyAttendance" || // CMT
                 p.SystemName == "MyEvaluation" || p.SystemName == "MyResult" || // PET
                 p.SystemName == "AccessRptMyResult" // PET
                 )
                 .Select(p => p.Id)
                 .ToList();

            return instructorPermissions;
        }
    }
}
