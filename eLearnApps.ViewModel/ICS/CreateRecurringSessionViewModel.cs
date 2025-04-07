using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text;
using System.Web;
using eLearnApps.Core;

namespace eLearnApps.ViewModel.ICS
{
    public class CreateRecurringSessionViewModel : IValidatableObject
    {
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<SessionTiming> Timings { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Validate start date
            if (DateTime.Compare(StartDate.Value, EndDate.Value) >= 0)
                yield return new ValidationResult("Start Date must be earlier than End Date.", new[] { "StartDate" });

            if (!DateTime.TryParse(StartDate.Value.ToString("dd MMM yyyy"), out _))
                yield return new ValidationResult("Start Date is invalid.", new[] { "StartDate" });

            if (DateTime.Compare(StartDate.Value.Date, DateTime.Now.Date) < 0)
                yield return new ValidationResult("Start Date is invalid.", new[] { "StartDate" });


            //Validate End date
            if (!DateTime.TryParse(EndDate.Value.ToString("dd MMM yyyy"), out _))
                yield return new ValidationResult("End Date is invalid.", new[] { "EndDate" });

            if (DateTime.Compare(EndDate.Value.Date, DateTime.Now.Date) < 0)
                yield return new ValidationResult("End Date is invalid.", new[] { "EndDate" });

            if (!string.IsNullOrEmpty(Title))
            {
                int length = Encoding.UTF8.GetByteCount(HttpUtility.UrlDecode(Title));
                if (length > 100)
                    yield return new ValidationResult("Title is not valid, maximum 100 characters.", new[] { "Title" });
            }

            //Validate timing
            var minTimeSpan = new TimeSpan(0, 0, 0);
            var maxTimeSpan = new TimeSpan(23, 59, 0);

            if (Timings == null || Timings.Count == 0)
                yield return new ValidationResult("There must be at least 1 day with start and end time.",
                    new[] { "Timings" });
            else
            {
                foreach (var timing in Timings)
                {
                    var day = timing.Day;
                    if (timing.StartTime.HasValue && timing.EndTime.HasValue)
                    {
                        if (timing.StartTime.Value < minTimeSpan || timing.StartTime.Value > maxTimeSpan)
                            yield return new ValidationResult($"<b>{day}</b>: Start time is not valid.", new[] { $"RecurringSession.Timings[{(int)day}].StartTime" });

                        if (timing.EndTime.Value < minTimeSpan || timing.EndTime.Value > maxTimeSpan)
                            yield return new ValidationResult($"<b>{day}</b>: End time is not valid.", new[] { $"RecurringSession.Timings[{(int)day}].EndTime" });

                        // Start time must be earlier than end time
                        if (timing.StartTime.Value >= timing.EndTime.Value)
                        {
                            yield return new ValidationResult($"<b>{day}</b>: Start Time must be earlier than End Time.", new[] { $"RecurringSession.Timings[{(int)day}].StartTime" });
                        }
                        else
                        {
                            // Interval must be between 15 minutes to 4 hours
                            var interval = timing.EndTime.Value - timing.StartTime.Value;
                            if (interval.TotalSeconds < Constants.MinIcsSessionLength || interval.TotalSeconds > Constants.MaxIcsSessionLength)
                            {
                                yield return new ValidationResult($"<b>{day}</b>: Interval must be between {Constants.MinIcsSessionLength / 60} minutes to {Constants.MaxIcsSessionLength / 3600} hours.", new[] { "Timings" });
                            }
                            else
                            {
                                // Each day with timing must have at least 1 assignment
                                if (Util.CountDays(day, StartDate!.Value, EndDate!.Value) == 0)
                                    yield return new ValidationResult($"<b>{day}</b>: No session between Start/End Date.", new[] { "Timings" });
                            }
                        }
                    }
                }
            }
        }
    }
}