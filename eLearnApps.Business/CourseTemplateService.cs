using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps.Business
{
    public class CourseTemplateService : ICourseTemplateService
    {
        private readonly IRepository<CourseTemplate> _courseTemplateRepository;

        public CourseTemplateService(IServiceProvider serviceProvider)
        {
            _courseTemplateRepository = serviceProvider.GetRequiredKeyedService<IRepository<CourseTemplate>>("default");
        }

        public CourseTemplate GetById(int id)
        {
            return _courseTemplateRepository.GetById(id);
        }

        public void Insert(CourseTemplate courseTemplate)
        {
            _courseTemplateRepository.Insert(courseTemplate);
        }

        public void Update(CourseTemplate course)
        {
            _courseTemplateRepository.Update(course);
        }

        public void Delete(CourseTemplate courseTemplate)
        {
            _courseTemplateRepository.Delete(courseTemplate);
        }

        public void Insert(List<CourseTemplate> courseTemplates)
        {
            _courseTemplateRepository.Insert(courseTemplates);
        }

        public void Save(List<CourseTemplate> courseTemplates)
        {
            var itemToCompareQuery = from nct in courseTemplates
                                     join ctr in _courseTemplateRepository.TableNoTracking on nct.Id equals ctr.Id into dbct
                                     from ct in dbct.DefaultIfEmpty()
                                     select new { incoming = nct, existing = ct };

            var itemsToCompare = itemToCompareQuery.ToList();
            var newItem = new List<CourseTemplate>();
            var itemToUpdate = new List<CourseTemplate>();
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

            if (newItem.Count > 0) _courseTemplateRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _courseTemplateRepository.Update(itemToUpdate);
        }
    }
}