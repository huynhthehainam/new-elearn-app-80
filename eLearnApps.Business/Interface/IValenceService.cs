using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.Security;

namespace eLearnApps.Business.Interface
{
    public interface IValenceService
    {
        void SyncEnrollmentOnly(List<int> courseIds);
        void SyncEnrollment(int userId, int courseId);

        /// <summary>
        /// Used by ta enrollment widget to find userid from email
        /// </summary>
        /// <param name="email">email of user</param>
        /// <returns>UserDataViewModel of object</returns>
        List<eLearnApps.ViewModel.Valence.UserDataViewModel> FindUserByEmail(string email);

        ViewModel.Valence.EnrollmentData GetEnrollment(int orgUnitId, int userId);

        List<Role> GetRoles();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<Entity.Valence.Section> GetSectionByCourseId(int courseId);
    }
}