using eLearnApps.Entity.LmsTools;
using System.Collections.Generic;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IPeerFeedBackResponsesDao
    {
        void Insert(List<PeerFeedBackResponses> responses);
        void BulkInsert(List<PeerFeedBackResponses> responses);
        void Delete(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds, int? peerFeedBackQuestionId = null);
    }
}
