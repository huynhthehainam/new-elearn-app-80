using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;

namespace eLearnApps.ViewModel.ICS
{
    public class SessionEditViewModel : IValidatableObject
    {
        public string Title { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Session is not valid.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Session Date is require!")]
        public DateTime? SessionDate { get; set; }

        [Required(ErrorMessage = "Start is require!")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? StartTime { get; set; }

        [Required(ErrorMessage = "End is require!")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? EndTime { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var minTimeSpan = TimeSpan.Zero;
            var maxTimeSpan = new TimeSpan(23, 59, 0);

            // Validate Start Time
            if (StartTime is null || StartTime < minTimeSpan || StartTime > maxTimeSpan)
                yield return new ValidationResult("Start time is not valid.", new[] { "StartTime" });

            // Validate End Time
            if (EndTime is null || EndTime < minTimeSpan || EndTime > maxTimeSpan)
                yield return new ValidationResult("End time is not valid.", new[] { "EndTime" });

            // Ensure StartTime < EndTime
            if (StartTime.HasValue && EndTime.HasValue && StartTime >= EndTime)
                yield return new ValidationResult("Start Time must be earlier than End Time.", new[] { "StartTime" });

            // Validate Session Date
            if (SessionDate.HasValue)
            {
                if (!DateTime.TryParseExact(SessionDate.Value.ToString("dd MMM yyyy"), "dd MMM yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    yield return new ValidationResult("Session Date is invalid.", new[] { "SessionDate" });
                }

                if (SessionDate.Value.Date < DateTime.Now.Date)
                    yield return new ValidationResult("Session Date must be today or later.", new[] { "SessionDate" });
            }

            // Validate Title
            if (!string.IsNullOrEmpty(Title))
            {
                int length = Encoding.UTF8.GetByteCount(WebUtility.UrlDecode(Title));
                if (length > 100)
                    yield return new ValidationResult("Title is not valid, maximum 100 characters.<br>", new[] { "Title" });
            }
        }
    }
}