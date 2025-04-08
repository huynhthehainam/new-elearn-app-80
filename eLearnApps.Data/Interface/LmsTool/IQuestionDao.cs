using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IQuestionDao
    {
        /// <summary>
        ///     Get question by list session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<Question> GetByListSession(List<string> ids);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        Task UpdateQuestionAddressed(Question question);
    }
}