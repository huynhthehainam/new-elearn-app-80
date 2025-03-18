using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.Security;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps.Business
{
    public class SemesterService : ISemesterService
    {
        private readonly IRepository<Semester> _semesterRepository;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Role> _roleRepository;

        public SemesterService(IServiceProvider serviceProvider)
        {
            _semesterRepository = serviceProvider.GetRequiredKeyedService<IRepository<Semester>>("default");
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default");
            _courseRepository = serviceProvider.GetRequiredKeyedService<IRepository<Course>>("default");
            _roleRepository = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");
        }

        public Semester GetById(int id)
        {
            return _semesterRepository.GetById(id);
        }

        public void Insert(Semester semester)
        {
            _semesterRepository.Insert(semester);
        }

        public void Update(Semester semester)
        {
            _semesterRepository.Update(semester);
        }

        public void Delete(Semester semester)
        {
            _semesterRepository.Delete(semester);
        }

        public void Insert(List<Semester> semesters)
        {
            _semesterRepository.Insert(semesters);
        }

        public List<Semester> GetAll() => _semesterRepository.Table.ToList();
        public List<Semester> GetAllByUserId(int userId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semester = _semesterRepository.Table;

            var query = from c in course
                        join s in semester on c.SemesterId equals s.Id
                        join e in enroll on c.Id equals e.CourseId
                        where e.UserId == userId
                        select s;

            return query.Distinct().ToList();
        }
        public async Task<List<Semester>> GetAllByUserIdAsync(int userId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semester = _semesterRepository.Table;

            var query = (from c in course
                         join s in semester on c.SemesterId equals s.Id
                         join e in enroll on c.Id equals e.CourseId
                         where e.UserId == userId
                         select s).OrderByDescending(s => s.Id).Distinct();

            return await query.ToListAsync();
        }

        public void Save(List<Semester> semesters)
        {
            var itemToCompareQuery = from ns in semesters
                                     join cr in _semesterRepository.TableNoTracking on ns.Id equals cr.Id into dbSemester
                                     from c in dbSemester.DefaultIfEmpty()
                                     select new { incoming = ns, existing = c };

            var itemsToCompare = itemToCompareQuery.ToList();
            var newItem = new List<Semester>();
            var itemToUpdate = new List<Semester>();
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

            if (newItem.Count > 0) _semesterRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _semesterRepository.Update(itemToUpdate);
        }
    }
}