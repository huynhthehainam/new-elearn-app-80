namespace eLearnApps.Data.Interface.Logging
{
    public interface ILoggingDaoFactory
    {
        IAuditLogDao AuditLogDao { get; }
        IGPTAuditLogDao GPTAuditLogDao { get; }
        IGPTDebugLogDao GPTDebugLogDao { get; }
        IBatchJobLogDao BatchJobLogDao { get; }
        IBatchJobLogDetailDao BatchJobLogDetailDao { get; }
        IErrorLogDao ErrorLogDao { get; }
        IToolAccessLogDao ToolAccessLogDao { get; }
        IDebugLogDao DebugLogDao { get; }
        IAuditDao AuditDao { get; }
    }
}