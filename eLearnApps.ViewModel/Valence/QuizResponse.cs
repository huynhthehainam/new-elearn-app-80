using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class UserResponse
    {
        public int StudentId { get; set; }

        public bool HasResponse { get; set; }

        public long QuizId { get; set; }

        public string QuizName { get; set; }

        public long AttemptId { get; set; }

        public List<Event> Events { get; set; }

        public List<Response> Responses { get; set; }

        public DateTimeOffset TimeStarted { get; set; }

        public DateTimeOffset TimeCompleted { get; set; }

        public long UserId { get; set; }

        public string SectionName { get; set; }

        public UserResponse()
        {
            HasResponse = true;
        }
    }

    public class Event
    {
        public string EventDescription { get; set; }

        public DateTimeOffset EventTime { get; set; }
    }

    public class Response
    {
        public long QuestionNumber { get; set; }

        public string QuestionText { get; set; }

        public string TextResponse { get; set; }

        public long? AnswerNumber { get; set; }

        public string QuestionType { get; set; }
        private bool isConverted = false;
        public bool GetIsConverted() => isConverted;
        public void SetIsConverted(bool value) => isConverted = value;
    }
}