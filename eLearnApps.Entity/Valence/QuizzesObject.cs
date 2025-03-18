using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{

    // 'Text' and 'Description' are property names used in their WS response but since those classes
    // already exist, we will rename it to Text2 and Description 
    public class QuizzesObject
    {
        public List<QuizObject> Objects { get; set; }
        public object Next { get; set; }

        public QuizzesObject() { Objects = new List<QuizObject>(); Next = null; }

    }

    public class Instructions
    {
        public Text2 Text { get; set; }
        public bool IsDisplayed { get; set; }
    }

    public class Text2
    {
        public string Text { get; set; }
        public string Html { get; set; }
    }

    public class Description2
    {
        public Text2 Text { get; set; }
        public bool IsDisplayed { get; set; }
    }

    public class SubmissionTimeLimit
    {
        public bool IsEnforced { get; set; }
        public bool ShowClock { get; set; }
        public int TimeLimitValue { get; set; }
    }

    public class LateSubmissionInfo
    {
        public int LateSubmissionOption { get; set; }
        public int? LateLimitMinutes { get; set; }
    }

    public class AttemptsAllowed
    {
        public bool IsUnlimited { get; set; }
        public int NumberOfAttemptsAllowed { get; set; }
    }

    public class QuizObject
    {
        public int QuizId { get; set; }
        public string Name { get; set; }
        public bool AutoExportToGrades { get; set; }
        public bool IsActive { get; set; }
        public object GradeItemId { get; set; }
        public bool IsAutoSetGraded { get; set; }
        public Instructions Instructions { get; set; }
        public Description2 Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool DisplayInCalendar { get; set; }
        public int SortOrder { get; set; }
        public SubmissionTimeLimit SubmissionTimeLimit { get; set; }
        public int SubmissionGracePeriod { get; set; }
        public LateSubmissionInfo LateSubmissionInfo { get; set; }
        public AttemptsAllowed AttemptsAllowed { get; set; }
        public string Password { get; set; }
    }
}