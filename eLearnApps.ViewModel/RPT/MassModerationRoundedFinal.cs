namespace eLearnApps.ViewModel.RPT
{
    public class MassModerationRoundedFinal
    {
        public int No { get; set; }
        public string Name { get; set; }
        public double? Marks { get; set; }
        public double? MarksOriginal { get; set; }
        public string Grade { get; set; }
        public string GradeOriginal { get; set; }

        public string Original
        {
            get
            {
                if (MarksOriginal.HasValue)
                    return $"{MarksOriginal} ({GradeOriginal})";
                else
                    return string.Empty;
            }
        }
        public string Moderated
        {
            get
            {
                if (Marks.HasValue)
                    return $"{Marks} ({Grade})";
                else
                    return string.Empty;
            }
        }

        public int Rank { get; set; }
        public string Color { get; set; }
    }
}