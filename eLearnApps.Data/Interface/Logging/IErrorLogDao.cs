using eLearnApps.Entity.Logging;
using System.Collections.Generic;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IErrorLogDao
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