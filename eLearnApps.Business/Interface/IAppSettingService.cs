using eLearnApps.Entity.LmsTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Business.Interface
{
    public interface IAppSettingService
    {
        AppSetting? GetById(int id);
        void Insert(AppSetting appSetting);
        void Update(AppSetting appSetting);
        void Delete(AppSetting appSetting);
        void Insert(List<AppSetting> appSettings);
        AppSetting? GetByKey(string key);
        IList<AppSetting> GetAllAppSettings();
    }
}
