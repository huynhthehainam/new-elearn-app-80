using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps.Business
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentService(IServiceProvider serviceProvider)
        {
            _departmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<Department>>("default");
        }

        public Department GetById(int id)
        {
            return _departmentRepository.GetById(id);
        }

        public void Insert(Department department)
        {
            _departmentRepository.Insert(department);
        }

        public void Update(Department department)
        {
            _departmentRepository.Update(department);
        }

        public void Delete(Department department)
        {
            _departmentRepository.Delete(department);
        }

        public void Insert(List<Department> departments)
        {
            _departmentRepository.Insert(departments);
        }

        public void Save(List<Department> departments)
        {
            var itemToCompareQuery = from newDept in departments
                                     join deptRepo in _departmentRepository.TableNoTracking on newDept.Id equals deptRepo.Id into dbDep
                                     from dept in dbDep.DefaultIfEmpty()
                                     select new { incoming = newDept, existing = dept };

            var itemsToCompare = itemToCompareQuery.ToList();
            var newItem = new List<Department>();
            var itemToUpdate = new List<Department>();
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

            if (newItem.Count > 0) _departmentRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _departmentRepository.Update(itemToUpdate);
        }
    }
}