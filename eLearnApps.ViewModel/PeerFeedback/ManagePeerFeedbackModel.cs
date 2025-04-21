using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.PeerFeedback
{
    
    public class ManagePeerFeedbackModel
    {
        public int? Id { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}