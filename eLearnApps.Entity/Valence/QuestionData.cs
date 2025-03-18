using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class QuestionData
    {
        public int QuestionId { get; set; }
        public QuestionType QuestionTypeId { get; set; }
        public int Points { get; set; }
        public int Difficulty { get; set; }
        public bool Bonus { get; set; }
        public bool Mandatory { get; set; }
        public RichTextInput Hint { get; set; }
        public RichTextInput Feedback { get; set; }
        public DateTime LastModified { get; set; }
        public long LastModifiedBy { get; set; }
        public long SectionId { get; set; }
        public long QuestionTemplateId { get; set; }
        public long QuestionTemplateVersionId { get; set; }
        public QuestionInfo QuestionInfo { get; set; }
        public RichTextInput QuestionText { get; set; }
    }
    public enum QuestionType
    {
        [Description("Multiple Choice")]
        MultipleChoice = 1,
        TrueFalse = 2,
        [Description("Fill in the Blanks")]
        FillInTheBlank = 3,
        [Description("Multi Select")]
        MultiSelect = 4,
        [Description("Matching")]
        Matching = 5,
        [Description("Ordering")]
        Ordering = 6,
        [Description("Long Answer")]
        LongAnswer = 7,
        [Description("Short Answer")]
        ShortAnswer = 8,
        [Description("Likert")]
        Likert = 9,
        [Description("Image Info")]
        ImageInfo = 10,
        [Description("Text Info")]
        TextInfo = 11,
        [Description("Arithmetic")]
        Arithmetic = 12,
        [Description("Significant Figures")]
        SignificantFigures = 13,
        [Description("Multi-Short Answer")]
        MultiShortAnswer = 14
    }

    public abstract class QI
    {

    }

    public class MultipleChoiceQI : QI
    {
        public List<MultipleChoiceAnswer> Answers { get; set; }
        public bool Randomize { get; set; }
        public int Enumeration { get; set; }
    }

    public class MultipleChoiceAnswer
    {
        public long PartId { get; set; }
        public RichTextInput Answer { get; set; }
        public RichTextInput AnswerFeedback { get; set; }
        public int Weight { get; set; }
    }

    public class TrueFalseQI : QI
    {
        public long TruePartId { get; set; }
        public int TrueWeight { get; set; }
        public RichTextInput TrueFeedback { get; set; }
        public long FalsePartId { get; set; }
        public int FalseWeight { get; set; }
        public RichTextInput FalseFeedback { get; set; }
        public int Enumeration { get; set; }
    }

    public class FIBQI : QI
    {
        public List<RichTextInput> Texts { get; set; }
        public List<Blank> Blanks { get; set; }
    }

    public class Blank
    {
        public long PartId { get; set; }
        public int Size { get; set; }
        public List<FIBAnswer> Answers { get; set; }
    }

    public class FIBAnswer
    {
        public string TextAnswer { get; set; }
        public int Weight { get; set; }
        public int EvaluationType { get; set; }
    }

    public class QuestionInfo
    {
        public bool Randomize { get; set; }
        public int Enumeration { get; set; }
        public int Style { get; set; }
        public int GradingType { get; set; }
        public long PartId { get; set; }
        public bool EnableStudentEditor { get; set; }
        public RichTextInput InitialText { get; set; }
        public RichTextInput AnswerKey { get; set; }
        public bool EnableAttachments { get; set; }
        public List<SABlank> Blanks { get; set; }
        public int Scale { get; set; }
        public bool NaOption { get; set; }
        public List<Statement_T> Statements { get; set; }
        public List<long> PartIds { get; set; }
        public int Boxes { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<QuestionInfoAnswer> Answers { get; set; }
    }
    public class QuestionInfoAnswer
    {
        public long PartId { get; set; }
        public RichTextInput Answer { get; set; }
        public RichTextInput AnswerFeedback { get; set; }
        public bool IsCorrect { get; set; }
        public string TextAnswer { get; set; }
        public int Weight { get; set; }
        public int EvaluationType { get; set; }
    }
    public class MultiSelectQI : QI
    {
        public bool Randomize { get; set; }
        public int Enumeration { get; set; }
        public int Style { get; set; }
        public int GradingType { get; set; }
        public List<MultiSelectAnswer> Answers { get; set; }
    }

    public class MultiSelectAnswer
    {
        public long PartId { get; set; }
        public RichTextInput Answer { get; set; }
        public RichTextInput AnswerFeedback { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class LongAnswerQI : QI
    {
        public long PartId { get; set; }
        public bool EnableStudentEditor { get; set; }
        public RichTextInput InitialText { get; set; }
        public RichTextInput AnswerKey { get; set; }
        public bool EnableAttachments { get; set; }
    }

    public class ShortAnswerQI : QI
    {
        public int GradingType { get; set; }
        public List<SABlank> Blanks { get; set; }
    }

    public class SABlank
    {
        public long PartId { get; set; }
        public int EvaluationType { get; set; }
        public List<FIBAnswer> Asnwers { get; set; }
    }

    public class LinkertQI : QI
    {
        public int Scale { get; set; }
        public bool NaOption { get; set; }
        public List<Statement_T> Statements { get; set; }
    }

    public class Statement_T
    {
        public long PartId { get; set; }
        public RichTextInput Statement { get; set; }
    }

    public class MultiShortAnswerQI : QI
    {
        public List<long> PartIds { get; set; }
        public int Boxes { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<FIBAnswer> Answers { get; set; }
    }

}
