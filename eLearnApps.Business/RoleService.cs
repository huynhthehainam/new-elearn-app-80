#region USING
using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;
#endregion


namespace eLearnApps.Business
{
    public class RoleService : IRoleService
    {
        #region REPOSITORY
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        #endregion

        #region CTOR
        public RoleService(
      IServiceProvider serviceProvider)
        {
            _roleRepository = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default"); ;
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default"); ;
        }
        #endregion

        public Role GetById(int id)
        {
            return _roleRepository.GetById(id);
        }

        public void Insert(Role role)
        {
            _roleRepository.Insert(role);
        }

        public void Update(Role role)
        {
            _roleRepository.Update(role);
        }

        public void Delete(Role role)
        {
            _roleRepository.Delete(role);
        }

        public void Insert(List<Role> roles)
        {
            _roleRepository.Insert(roles);
        }

        public void Save(List<Role> roles)
        {
            var itemToCompareQuery = from newrole in roles
                                     join roleRepo in _roleRepository.TableNoTracking on newrole.Id equals roleRepo.Id into dbRole
                                     from role in dbRole.DefaultIfEmpty()
                                     select new { incoming = newrole, existing = role };

            var itemsToCompare = itemToCompareQuery.ToList();
            var newItem = new List<Role>();
            var itemToUpdate = new List<Role>();
            foreach (var item in itemsToCompare)
            {
                if (item.existing == null) newItem.Add(item.incoming);
                else
                {
                    if (item.existing.Name != item.incoming.Name || item.existing.Code != item.incoming.Code)
                    {
                        var toupdate = item.existing;
                        toupdate.Name = item.incoming.Name;
                        toupdate.Code = item.incoming.Code;
                        itemToUpdate.Add(toupdate);
                    }
                }
            }

            if (newItem.Count > 0) _roleRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _roleRepository.Update(itemToUpdate);
        }

        public List<Role> GetRoles()
        {
            return _roleRepository.Table.ToList();
        }

        public List<Role> FindByIds(List<int?> ids)
        {
            return _roleRepository.Table.Where(x => ids.IndexOf(x.Id) > -1).ToList();
        }
        /// <summary>
        /// Get role by userid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Role> GetRoleByUserId(int userId)
        {
            var query = _roleRepository.Table;
            var queryUserEnroll = _userEnrollmentRepository.Table.Where(x => x.UserId == userId).Select(x => x.RoleId);
            var result = query.Where(x => queryUserEnroll.Contains(x.Id));
            return result.ToList();
        }
    }
}