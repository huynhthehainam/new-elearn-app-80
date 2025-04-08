using System.Collections.Generic;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface ILearningPointDao
    {
        /// <summary>
        ///     Get learning point by list session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<LearningPointDto> GetByListSession(List<string> ids);
    }
}