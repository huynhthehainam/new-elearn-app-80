using eLearnApps.Entity.Logging;
using System.Collections.Generic;

namespace eLearnApps.Business.Interface
{
    public interface IErrorLogService
    {
        /// <summary>
        ///     Insert ErrorLog
        /// </summary>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        int Insert(ErrorLog errorLog);
        List<ErrorLog> GetAll();
    }
}