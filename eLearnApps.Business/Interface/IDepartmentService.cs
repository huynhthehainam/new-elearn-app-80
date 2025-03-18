using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Business.Interface
{
    public interface IDepartmentService
    {
        Department GetById(int id);
        void Insert(Department course);
        void Update(Department course);
        void Delete(Department course);
        void Insert(List<Department> courses);
        void Save(List<Department> courses);
    }
}