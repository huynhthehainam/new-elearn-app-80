using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using eLearnApps.ViewModel.PET;

namespace eLearnApps.ViewModel.PeerFeedback
{

    public class PeerFeedbackSessionViewModel
    {
        public int? Id { get; set; }

        public DateTime EntryStartTime { get; set; }

        public DateTime EntryCloseTime { get; set; }
        public string? Strm { get; set; }
        public string? Label { get; set; }
        public int? PeerFeedbackId { get; set; }
        public double StartTotalMilliseconds { get; set; }
        public double EndTotalMilliseconds { get; set; }
        public string? Term { get; set; }
        public List<Item> Targets { get; set; } = new List<Item>();
        public PeerFeedbackResultSessionStatus PeerFeedbackResultSessionStatus { get; set; }
        public string? PeerFeedBackKey { get; set; }
        public Double Progress { get; set; }
        public List<string> CourseOfferingCode { get; set; } = new List<string>();
    }

    public class PeerFeedbackSessionAddOrEditViewModel
    {
        [Required]
        public PeerFeedbackSessionViewModel Session { get; set; } = new PeerFeedbackSessionViewModel();
        public List<DefaultViewModel> Terms { get; set; }
    }

    public enum PeerFeedbackResultSessionStatus
    {
        Ongoing = 1,
        Closed = 2,
        Incomplete = 3
    }
}