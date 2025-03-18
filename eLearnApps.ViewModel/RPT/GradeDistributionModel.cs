namespace eLearnApps.ViewModel.RPT
{
    public class GradeDistributionModel
    {
        public string Grade { set; get; }
        public string Marks { set; get; }
        public int CountValue { get; set; }
        public double PercentValue { get; set; }
        public double AccumValue { set; get; }
        public string Count { set; get; }
        public string Percent { set; get; }
        public string Accum { set; get; }
        public string Color { set; get; }
        public int No { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }
        public string DisplayGrade => SectionRowNumber > 0 ? string.Empty : Grade;
        public string DisplayMark => SectionRowNumber > 0 ? string.Empty : Marks;
        public int SectionRowNumber { get; set; }
        public bool IsLastRowOfSection { get; set; }

    }
}