using eLearnApps.Entity.LmsTools;
using System.Collections.Generic;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IPeerFeedBackResponseRemarksDao
    {
        void Insert(PeerFeedBackResponseRemarks remarks);
        void Update(PeerFeedBackResponseRemarks remarks);
        void BulkInsert(List<PeerFeedBackResponseRemarks> remarks);
        void Delete(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds);
    }
}
