namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationTypeProvider
    {
        public static readonly EvaluationType StudentEvaluateOneAnother = new EvaluationType
        {
            EvaluationTypeId =  1, Name = "Students evaluate one another",
            Description =
                "This allows students to evaluate one another. The students may self-evaluate if self-evaluation option is enabled. "
        };

        public static readonly EvaluationType StudentEvaluateOwnGroupMember = new EvaluationType
        {
            EvaluationTypeId =  2, Name = "Students evaluate own group members",
            Description =
                "This allows students to evaluate their own group members. The students can evaluate themselves if self-evaluation option is enabled. "
        };

        public static readonly EvaluationType StudentEvaluateGroup = new EvaluationType
        {
            EvaluationTypeId =  3, Name = "Students evaluate groups",
            Description =
                "This allows students to evaluate groups. Members of the groups receive the same scores and comments. The students can evaluate their own group if self-evaluation option is enabled."
        };

        public static readonly EvaluationType StudentSelfEvaluate = new EvaluationType
        {
            EvaluationTypeId =  4, Name = "Students self-evaluate",
            Description =
                "This allows students to self-evaluate. Only instructors can view the students' self-evaluations."
        };

        public static readonly EvaluationType StudentEvaluateTas = new EvaluationType
        {
            EvaluationTypeId =  5, Name = "Students evaluate TAs",
            Description =
                "This allows students to evaluate teaching assistants (TAs). TAs are not able to access this type of evaluations."
        };

        public static readonly EvaluationType InstructorEvaluateStudent = new EvaluationType
        {
            EvaluationTypeId =  6, Name = "Instructors evaluate students",
            Description = "This allows instructors to evaluate students. "
        };

        public static readonly EvaluationType InstructorEvaluateGroup = new EvaluationType { EvaluationTypeId =  7, Name = "Instructors evaluate groups",
            Description = "This allows instructors to evaluate groups of students.  Members of the groups receive the same scores and comments." };
    }
}