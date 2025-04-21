namespace eLearnApps.Data.Interface.Lmsisis
{
    public interface ILmsisDaoFactory
    {
        //ITLMergeSectionDao TLMergeSectionDao { get; }
        ITLCourseOfferingDao TLCourseOfferingDao { get; }
        //IPsSisLmsGradeTypeDao PsSisLmsGradeTypeDao { get; }
        IPS_SIS_LMS_PHOTO_VDao PS_SIS_LMS_PHOTO_VDao { get; }
    }
}