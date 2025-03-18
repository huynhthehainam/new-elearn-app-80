using eLearnApps.Entity.Logging;
using System.Collections.Generic;

namespace eLearnApps.Business.Interface
{
    public interface IGPTDebugLogService
    {
        /// <summary>
        ///     Insert GPTDebugLog
        /// </summary>
        /// <param name="gptDebugLog"></param>
        /// <returns></returns>
        int Insert(GPTDebugLog gptDebugLog);
        List<GPTDebugLog> GetAll();
    }
}