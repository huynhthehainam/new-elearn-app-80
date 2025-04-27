using eLearnApps.Entity.LmsTools;
using System.Collections.Generic;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IPeerTutoringRecordDao
    {
        void Insert(PeerTutoringRecord grades);
        void Update(PeerTutoringRecord newRecord);
        List<PeerTutoringRecord> GetRecordByKey(int courseId, int studentUserId);
        List<PeerTutoringRecord> GetRecordByKey(int courseId);
        bool CheckExistsRecord(int courseId, int studentUserId);
        PeerTutoringRecord GetPeerTutoringRecordByKey(int courseId, int studentUserId);
    }
}
