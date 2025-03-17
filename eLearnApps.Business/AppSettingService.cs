#region USING
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.DependencyInjection;


#endregion


namespace eLearnApps.Business
{
    public class AppSettingService : IAppSettingService
    {

        #region REPOSITORY
        private readonly IRepository<AppSetting> _appSettingRepository;


        #endregion

        #region CTOR

        public AppSettingService(
           IServiceProvider serviceProvider
)
        {

            _appSettingRepository = serviceProvider.GetRequiredKeyedService<IRepository<AppSetting>>("default");

        }

        #endregion

        public AppSetting? GetById(int id)
        {
            return _appSettingRepository.GetById(id);
        }

        public void Insert(AppSetting appSetting)
        {
            _appSettingRepository.Insert(appSetting);
        }

        public void Update(AppSetting appSetting)
        {
            _appSettingRepository.Update(appSetting);
        }

        public void Delete(AppSetting appSetting)
        {
            _appSettingRepository.Delete(appSetting);
        }

        public void Insert(List<AppSetting> evaluations)
        {
            _appSettingRepository.Insert(evaluations);
        }

        public AppSetting? GetByKey(string key)
        {
            return _appSettingRepository.Table.FirstOrDefault(x => x.Key == key);
        }
        public IList<AppSetting> GetAllAppSettings() => _appSettingRepository.Table.ToList();
    }
}