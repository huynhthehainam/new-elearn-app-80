using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface ILearningPointCheckDao
    {
        /// <summary>
        ///     Get list learning point check by list learning point id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<LearningPointCheck> GetByListLearningPoint(List<string> ids);
    }
}