namespace eLearnApps.ViewModel.Valence.Grade
{
    public class GradeIncomingGradeValue
    {
        public Comments Comments { get; set; }
        public Privatecomments PrivateComments { get; set; }
        public int GradeObjectType { get; set; }
        public decimal PointsNumerator { get; set; }
    }

    public class Comments
    {
        public string Content { get; set; }
        public string Type { get; set; }
    }

    public class Privatecomments
    {
        public string Content { get; set; }
        public string Type { get; set; }
    }
}
