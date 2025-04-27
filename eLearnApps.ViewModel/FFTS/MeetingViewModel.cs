using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.FFTS
{
    public class MeetingViewModel : IValidatableObject
    {
        public int? MeetingID { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot be longer than 1000 characters.")]
        public string Description { get; set; }
        [StringLength(200, ErrorMessage = "Course cannot be longer than 200 characters.")]
        public string Course { get; set; }
        [StringLength(200, ErrorMessage = "Location cannot be longer than 200 characters.")]
        public string Location { get; set; }

        public int? OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Start { get; set; }

        public int? Type { get; set; }

        [DataType(DataType.Date)]
        public DateTime? End { get; set; }

        public List<Attendee> Attendees { get; set; }
        public List<int> AttendeeIds { get; set; }

        public double StartD { get; set; }
        public double EndD { get; set; }
        public int CourseId { get; set; }
        public string MeetingKey { get; set; }
        public bool CanEdit { get; set; }
        public int? InviteStatus { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateTime.Compare(Start.Value, End.Value) > 0) yield return new ValidationResult("StartDate, EndDate is not valid.");
        }
    }
}