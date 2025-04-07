using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.EE
{
    public class QuestionViewModel : IEquatable<QuestionViewModel>
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }

        public QuestionViewModel(string text, string type)
        {
            QuestionText = text;
            QuestionType = type;
        }

        public bool Equals(QuestionViewModel other)
        {
            if (QuestionText == other.QuestionText && QuestionType == other.QuestionType)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashQuestionText = QuestionText == null ? 0 : QuestionText.GetHashCode();
            int hashQuestionType = QuestionType == null ? 0 : QuestionType.GetHashCode();

            return hashQuestionText ^ hashQuestionType;
        }
    }
}
