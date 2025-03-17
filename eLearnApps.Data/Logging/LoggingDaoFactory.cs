using eLearnApps.Data.Interface.Logging;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.Logging
{
    public class LoggingDaoFactory : ILoggingDaoFactory
    {
        private readonly IConfiguration _configuration;
        public LoggingDaoFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IAuditLogDao AuditLogDao => new AuditLogDao(_configuration);
        public IBatchJobLogDao BatchJobLogDao => new BatchJobLogDao(_configuration);
        public IBatchJobLogDetailDao BatchJobLogDetailDao => new BatchJobLogDetailDao(_configuration);
        public IErrorLogDao ErrorLogDao => new ErrorLogDao(_configuration);
        public IToolAccessLogDao ToolAccessLogDao => new ToolAccessLogDao(_configuration);
        public IGPTDebugLogDao GPTDebugLogDao => new GPTDebugLogDao(_configuration);
        public IGPTAuditLogDao GPTAuditLogDao => new GPTAuditLogDao(_configuration);
        public IDebugLogDao DebugLogDao => new DebugLogDao(_configuration);
        public IAuditDao AuditDao => new AuditDao(_configuration);
    }
}