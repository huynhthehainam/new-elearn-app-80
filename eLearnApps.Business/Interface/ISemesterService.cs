using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Business.Interface
{
    public interface ISemesterService
    {
        Semester GetById(int id);
        void Insert(Semester semester);
        void Update(Semester semester);
        void Delete(Semester semester);
        void Insert(List<Semester> semesters);
        List<Semester> GetAll();
        List<Semester> GetAllByUserId(int userId);
        void Save(List<Semester> semesters);
        Task<List<Semester>> GetAllByUserIdAsync(int userId);
    }
}