using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using System.Collections.Generic;

namespace eLearnApps.Business
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IErrorLogDao _errorLogDao;

        public ErrorLogService(IDaoFactory factory)
        {
            _errorLogDao = factory.ErrorLogDao;
        }

        /// <summary>
        ///     Insert error log
        /// </summary>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        public int Insert(ErrorLog errorLog)
        {
            return _errorLogDao.Insert(errorLog);
        }

        /// <summary>
        /// Get all error logs
        /// </summary>
        /// <returns></returns>
        public List<ErrorLog> GetAll()
        {
            return _errorLogDao.GetAll();
        }
    }
}