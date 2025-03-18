using System.Collections.Generic;
using System.Threading.Tasks;

namespace eLearnApps.Data.Interface.Lmsisis
{
    public interface IPS_SIS_LMS_PHOTO_VDao
    {
        List<string> GetAllEmpId();

        Entity.LmsIsis.StudentPhoto GetStudentPhotoByOrgDefineId(string orgDefinedId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgDefinedId"></param>
        /// <returns></returns>
        Task<Entity.LmsIsis.StudentPhoto> GetStudentPhotoByOrgDefineIdAsync(string orgDefinedId);
    }
}