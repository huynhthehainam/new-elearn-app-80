﻿using System;

namespace eLearnApps.Entity.LmsTools
{
    public class Log : BaseEntity
    {
        public int Id { get; set; }
        public string? ShortMessage { get; set; }
        public string? FullMessage { get; set; }
        public string? IpAddress { get; set; }
        public int? UserId { get; set; }
        public string? PageUrl { get; set; }
        public string? ReferrerUrl { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int LogLevel { get; set; }
    }
}