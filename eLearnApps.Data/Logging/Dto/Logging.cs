namespace eLearnApps.Entity.Logging.Dto
{
    public class Logging
    {
        public AuditLog? Audit { get; set; }
        public BatchJobLog? BatchJob { get; set; }
        public BatchJobLogDetail? BatchJobDetail { get; set; }
        public ErrorLog? Error { get; set; }
        public ToolAccessLog? ToolAccess { get; set; }
        public GPTDebugLog? GPTDebug { get; set; }
        public GPTAuditLog? GPTAudit { get; set; }
    }
}