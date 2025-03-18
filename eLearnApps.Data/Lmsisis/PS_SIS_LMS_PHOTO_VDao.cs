using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Lmsisis;
using eLearnApps.Entity.LmsIsis;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.Lmsisis
{
    public class PS_SIS_LMS_PHOTO_VDao : BaseDao, IPS_SIS_LMS_PHOTO_VDao
    {
        public PS_SIS_LMS_PHOTO_VDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Lmsisis;
        }

        /// <summary>
        /// Get all EMPLID in PS_SIS_LMS_PHOTO_V
        /// </summary>
        /// <returns>List of EMPLID in PS_SIS_LMS_PHOTO_V</returns>
        public List<string> GetAllEmpId()
        {
            try
            {
                var query = "SELECT EMPLID FROM PS_SIS_LMS_PHOTO_V";
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<string>(query,
                        commandType: CommandType.Text)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetStudentPhotoByOrgDefineIdQuery()
        {
            return @"SELECT EMPLID as PersonId, 1 as IdCardNo, EMPLOYEE_PHOTO as Photo FROM PS_SIS_LMS_PHOTO_V 
                              WHERE EMPLID IN (SELECT EMPLID FROM PS_SIS_LMS_STD_VW WHERE CAMPUS_ID = @OrgDefinedId)";
        }
        public StudentPhoto GetStudentPhotoByOrgDefineId(string orgDefinedId)
        {
            try
            {
                var query = GetStudentPhotoByOrgDefineIdQuery();
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = conn.Query<StudentPhoto>(query,
                        new
                        {
                            orgDefinedId
                        },
                        commandType: CommandType.Text)
                        .FirstOrDefault();

                    if (result != null)
                        result.IdCardNo = orgDefinedId;

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<StudentPhoto> GetStudentPhotoByOrgDefineIdAsync(string orgDefinedId)
        {
            try
            {
                var query = GetStudentPhotoByOrgDefineIdQuery();
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.QueryFirstOrDefaultAsync<StudentPhoto>(query,
                            new
                            {
                                orgDefinedId
                            },
                            commandType: CommandType.Text);

                    if (result != null)
                        result.IdCardNo = orgDefinedId;

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
