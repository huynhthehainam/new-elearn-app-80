using System;

namespace eLearnApps.ViewModel.FFTS
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public int LogLevel { get; set; }
        public string LogLevelText { get; set; }

public string ShortMessage { get; set; }

public string FullMessage { get; set; }

public string IpAddress { get; set; }

        public int? UserId { get; set; }

public string PageUrl { get; set; }

public string ReferrerUrl { get; set; }

        public DateTime CreatedOn { get; set; }
        public double TotalMilliseconds { get; set; }
        public string DisplayUserName { get; set; }
    }
}