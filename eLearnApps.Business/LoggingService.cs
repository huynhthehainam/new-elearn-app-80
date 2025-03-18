#region USING
using System;
using System.Threading.Tasks;
using System.Transactions;
using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Data.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using log4net;
using System.Reflection;
using eLearnApps.Data.Logging;
using eLearnApps.Entity.Logging.Dto;
#endregion

namespace eLearnApps.Business
{
    public class LoggingService : ILoggingService
    {
        #region SERVICE
        private readonly IAuditLogService _auditLogService;
        private readonly IGPTDebugLogService _gptDebugLogService;
        private readonly IGPTAuditLogService _gptAuditLogService;
        private readonly IBatchJobLogDetailService _batchJobLogDetailService;
        private readonly IBatchJobLogService _batchJobLogService;
        private readonly IErrorLogService _errorLogService;
        private readonly IToolAccessLogService _toolAccessLogService;
        private readonly IDebugLogDao _debugLogDao;
        private readonly IAuditDao _auditDao;
        private static readonly ILog fileLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region CTOR
        public LoggingService(IAuditLogService auditLogService
            , IGPTDebugLogService gptDebugLogService
            , IGPTAuditLogService gptAuditLogService
            , IBatchJobLogService batchJobLogService
            , IBatchJobLogDetailService batchJobLogDetailService
            , IErrorLogService errorLogService
            , IDaoFactory factory
            , IToolAccessLogService toolAccessLogService)
        {
            _auditLogService = auditLogService;
            _gptDebugLogService = gptDebugLogService;
            _gptAuditLogService = gptAuditLogService;
            _batchJobLogService = batchJobLogService;
            _batchJobLogDetailService = batchJobLogDetailService;
            _errorLogService = errorLogService;
            _toolAccessLogService = toolAccessLogService;

            _debugLogDao = factory.DebugLogDao;
            _auditDao = factory.AuditDao;
        }
        #endregion

        public int Error(int userId, string toolId, int orgUnitId, int roleId, string page, string errorMessage, string errorDetails,
            string IpAddress, string sessionid)
        {
            var result = _errorLogService.Insert(new Entity.Logging.ErrorLog()
            {
                UserId = userId,
                ToolId = toolId,
                OrgUnitId = orgUnitId,
                ToolAccessRoleId = roleId,
                ErrorPage = page,
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails,
                ErrorTime = System.DateTime.Now,
                IpAddress = IpAddress,
                SessionId = sessionid
            });

            fileLog.Error(errorMessage, new Exception(errorDetails));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toolId"></param>
        /// <param name="orgUnitId"></param>
        /// <param name="roleId"></param>
        /// <param name="actionCategory"></param>
        /// <param name="actionDescription"></param>
        /// <param name="actionTime"></param>
        /// <param name="ipAddress"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task Audit(int userId, string toolId, int orgUnitId, int roleId, string actionCategory, string actionDescription, DateTime actionTime, string ipAddress, string sessionId)
        {
            var audit = new Entity.Logging.AuditLog
            {
                ActionCategory = actionCategory,
                ActionTime = actionTime,
                ActionDescription = actionDescription,
                CreatedOn = DateTime.UtcNow,
                IpAddress = ipAddress,
                SessionId = sessionId,
                ToolAccessRoleId = roleId,
                OrgUnitId = orgUnitId,
                ToolId = toolId,
                UserId = userId
            };
            await _auditLogService.Insert(audit);
        }
        /// <summary>
        ///     Save log
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logging"></param>
        /// <returns></returns>
        public int Save(LogType logType, Logging logging)
        {
            var result = -1;
            switch (logType)
            {
                case LogType.BatchJob:
                    var batchJobLogId = _batchJobLogService.Insert(logging.BatchJob);
                    logging.BatchJobDetail.BatchJobLogId = batchJobLogId;
                    result = _batchJobLogDetailService.Insert(logging.BatchJobDetail);
                    break;
                case LogType.Error:
                    result = _errorLogService.Insert(logging.Error);
                    break;
                case LogType.TrackLog:
                    result = _toolAccessLogService.Insert(logging.ToolAccess);
                    break;
                case LogType.GPTDebug:
                    result = _gptDebugLogService.Insert(logging.GPTDebug);
                    break;
                case LogType.GPTAudit:
                    result = _gptAuditLogService.Insert(logging.GPTAudit);
                    break;
            }

            return result;
        }
        public async Task Debug(DebugLog debuglog)
        {
            await _debugLogDao.Insert(debuglog);
        }
        public void AuditUserAction(AuditEntry audit)
        {
            _auditDao.Insert(audit.ToAudit());
        }
    }
}