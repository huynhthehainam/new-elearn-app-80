using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using System.Collections.Generic;

namespace eLearnApps.Business
{
    public class GPTDebugLogService : IGPTDebugLogService
    {
        private readonly IGPTDebugLogDao _gptDebugLogDao;

        public GPTDebugLogService(IDaoFactory factory)
        {
            _gptDebugLogDao = factory.GPTDebugLogDao;
        }

        /// <summary>
        ///     Insert Audit log
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public int Insert(GPTDebugLog gptDebugLog)
        {
            return _gptDebugLogDao.Insert(gptDebugLog);
        }
        public List<GPTDebugLog> GetAll()
        {
            return _gptDebugLogDao.GetAll();
        }
    }
}