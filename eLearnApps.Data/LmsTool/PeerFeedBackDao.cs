using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace eLearnApps.Data.LmsTool
{
    public class PeerFeedBackDao : BaseDao, IPeerFeedBackDao
    {
        public IConfiguration Configuration { get; }

        public PeerFeedBackDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
            Configuration = configuration;
        }
        /// <summary>
        /// Delete PeerFeedBack
        /// will update delete flag in tables
        /// 1, PeerFeedback 
        /// 2, PeerFeedBackResponses
        /// 3, PeerFeedbackPairings
        /// 4, PeerFeedbackSessions
        /// 5, PeerFeedbackEvaluators
        /// 6, PeerFeedbackTargets
        /// </summary>
        /// <param name="peerFeedBackId"></param>
        /// <param name="lastUpdatedBy"></param>
        public void Delete(int peerFeedBackId, int lastUpdatedBy)
        {
            try
            {
                var sb = new System.Text.StringBuilder(1471);
                sb.AppendLine(@"BEGIN");
                sb.AppendLine(@"	BEGIN TRANSACTION;  ");
                sb.AppendLine(@"		--update flag delete PeerFeedBackResponses");
                sb.AppendLine(@"		UPDATE PeerFeedBackResponses SET IsDeleted = 1, LastUpdateTime = SYSUTCDATETIME() WHERE PeerFeedBackId = @PeerFeedBackId;");
                sb.AppendLine(@"		--update flag delete PeerFeedbackPairings");
                sb.AppendLine(@"		UPDATE PeerFeedbackPairings SET IsDeleted = 1, LastUpdatedBy = @LastUpdatedBy, LastUpdatedTime = SYSUTCDATETIME() WHERE PeerFeedBackId = @PeerFeedBackId;");
                sb.AppendLine(@"		--update flag delete PeerFeedbackEvaluators");
                sb.AppendLine(@"		UPDATE ");
                sb.AppendLine(@"			evaluators ");
                sb.AppendLine(@"		SET ");
                sb.AppendLine(@"			IsDeleted = 1, LastUpdatedBy = @LastUpdatedBy, LastUpdatedTime = SYSUTCDATETIME() ");
                sb.AppendLine(@"		FROM ");
                sb.AppendLine(@"			PeerFeedbackEvaluators evaluators INNER JOIN PeerFeedbackPairings pp on evaluators.PeerFeedbackPairingId = pp.Id");
                sb.AppendLine(@"		WHERE ");
                sb.AppendLine(@"			pp.PeerFeedBackId = @PeerFeedBackId;");
                sb.AppendLine(@"		--update flag delete PeerFeedbackTargets");
                sb.AppendLine(@"		UPDATE ");
                sb.AppendLine(@"			targets");
                sb.AppendLine(@"		SET ");
                sb.AppendLine(@"			IsDeleted = 1, LastUpdatedBy = @LastUpdatedBy, LastUpdatedTime = SYSUTCDATETIME() ");
                sb.AppendLine(@"		FROM ");
                sb.AppendLine(@"			PeerFeedbackTargets targets  INNER JOIN PeerFeedbackPairings pp on targets.PeerFeedbackPairingId = pp.Id");
                sb.AppendLine(@"		WHERE ");
                sb.AppendLine(@"			pp.PeerFeedBackId = @PeerFeedBackId;");
                sb.AppendLine(@"		--update flag delete PeerFeedbackSessions");
                sb.AppendLine(@"		UPDATE PeerFeedbackSessions SET IsDeleted = 1, LastUpdatedBy = @LastUpdatedBy, LastUpdatedTime = SYSUTCDATETIME() WHERE PeerFeedBackId = @PeerFeedBackId;");
                sb.AppendLine(@"		--update flag delete PeerFeedback");
                sb.AppendLine(@"		UPDATE PeerFeedback SET IsDeleted = 1, LastUpdatedBy = @LastUpdatedBy, LastUpdatedTime = SYSUTCDATETIME() WHERE Id = @PeerFeedBackId;");
                sb.AppendLine(@"	COMMIT;  ");
                sb.AppendLine(@"END");

                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sb.ToString(), new { peerFeedBackId, lastUpdatedBy },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="peerFeedbackId"></param>
        /// <param name="peerFeedBackSessionId"></param>
        /// <param name="courseOfferingCode"></param>
        /// <param name="whiteListedCourses"></param>
        /// <param name="updatingUserId"></param>
        public void GeneratePairings(int peerFeedbackId, int peerFeedBackSessionId, List<string> courseOfferingCode, int updatingUserId)
        {
            using (var conn = CreateConnection(ConnectionString))
            {
                conn.Execute(Resources.LmsTool.PeerFeedBack_GeneratePairings,
                    new
                    {
                        LastUpdatedBy = updatingUserId,
                        PeerFeedbackId = peerFeedbackId,
                        PeerFeedBackSessionId = peerFeedBackSessionId,
                        CourseOfferingCode = courseOfferingCode.ToArray(),
                    },
                    commandType: CommandType.Text);
            }
        }
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="dataItems"></param>
        /// <returns></returns>
        public async Task GeneratePairings(List<(PeerFeedbackPairings Pairing, List<PeerFeedBackPairingSessions> PairingSession, List<PeerFeedbackTargets> Targets, List<PeerFeedbackEvaluators> Evaluators)> dataItems)
        {
            if(dataItems == null || dataItems.Count == 0)
                throw new ArgumentNullException(nameof(dataItems));
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var item in dataItems)
                        {
                            //insert PeerFeedbackPairings
                            var pairingId = await conn.QueryFirstOrDefaultAsync<int>(Resources.LmsTool.PeerFeedBackPairings_Insert, item.Pairing, transaction, commandType: CommandType.Text);

                            //insert PeerFeedBackPairingSessions
                            var pairingSession = item.PairingSession.Select(x =>
                            {
                                x.PeerFeedBackPairingId = pairingId;
                                return x;
                            }).ToList();
                            await conn.ExecuteAsync(Resources.LmsTool.PeerFeedBackPairingSessions_Insert, pairingSession, transaction, commandType: CommandType.Text);

                            //insert PeerFeedbackTargets
                            var targets = item.Targets.Select(x =>
                            {
                                x.PeerFeedbackPairingId = pairingId;
                                return x;
                            }).ToList();
                            await conn.ExecuteAsync(Resources.LmsTool.PeerFeedBackTargets_Insert, targets, transaction, commandType: CommandType.Text);

                            //insert PeerFeedbackEvaluators
                            var evaluators = item.Evaluators.Select(x =>
                            {
                                x.PeerFeedbackPairingId = pairingId;
                                return x;
                            }).ToList();
                            await conn.ExecuteAsync(Resources.LmsTool.PeerFeedBackEvaluators_Insert, evaluators, transaction, commandType: CommandType.Text);
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PeerFeedBackGroupReadinessReportDto> GetGroupReadinessData(List<string> selectedCourseCodes)
        {
            ConnectionType = ConnectionStringType.DataHub;
            using (var conn = CreateConnection(ConnectionString))
            {
                return conn.Query<PeerFeedBackGroupReadinessReportDto>(Resources.Reports.group_readiness,
                    new
                    {
                        selectedCourseCodes = selectedCourseCodes.ToArray(),
                    },
                    commandType: CommandType.Text);
            }
        }
        public async Task<IEnumerable<CourseOfferingDto>> GetListCourseOfferingByCodes(AcadCareer acad_career, int pageNumber, int pageSize, string filter = "", bool useFullDbName = false)
        {
            try
            {
                var alias = useFullDbName ? $"{DatabaseName.LMSISIS.ToString()}.dbo." : "";
                if (!string.IsNullOrEmpty(filter))
                    filter = $"%{filter}%";

                string acadCareerValue = Constants.AcadCareerUGRD;
                string filterAcadCareer = GetFilterAcadCareerQuery(acad_career);

                string query = string.Format(Resources.LmsTool.TL_CourseOfferings_GetListByCodes, alias, filterAcadCareer);

                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.QueryAsync<CourseOfferingDto>(query,
                        new
                        {
                            //pageSize,
                            //pageNumber,
                            acad_career = acadCareerValue,
                            filter
                        }, commandType: CommandType.Text, commandTimeout: 120);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetFilterAcadCareerQuery(AcadCareer acad_career)
        {
            string filterAcadCareer = string.Empty;
            if (acad_career == AcadCareer.UG)
                filterAcadCareer = " AND LOWER(ACAD_CAREER) = LOWER (@ACAD_CAREER)";
            else if (acad_career == AcadCareer.PG)
                filterAcadCareer = " AND LOWER(ACAD_CAREER) != LOWER (@ACAD_CAREER)";
            return filterAcadCareer;
        }
        public async Task<int> GetTotalCountTlCourseOfferingByCodes(AcadCareer acad_career, string filter = "", bool useFullDbName = false)
        {
            try
            {
                var alias = useFullDbName ? $"{DatabaseName.LMSISIS.ToString()}.dbo." : "";
                if (!string.IsNullOrEmpty(filter))
                    filter = $"%{filter}%";

                string acadCareerValue = Constants.AcadCareerUGRD;
                string filterAcadCareer = GetFilterAcadCareerQuery(acad_career);

                string query = string.Format(Resources.LmsTool.TL_CourseOfferings_GetTotalCountByCodes, alias, filterAcadCareer);
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.ExecuteScalarAsync<int>(query,
                        new
                        {
                            filter,
                            acad_career = acadCareerValue
                        }, commandType: CommandType.Text);
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
