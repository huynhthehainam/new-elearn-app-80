namespace eLearnApps.Entity.LmsTools
{
    public class AttendanceListCategoryOrSection : BaseEntity
    {
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AttendanceListId { get; set; }
        /// <summary>
        /// CategoryId or SectionId
        /// </summary>
        public int CategoryOrSectionId { get; set; } 
        /// <summary>
        /// Category or Section
        /// </summary>
        public byte Type { get; set; } 
    }
}