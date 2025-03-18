using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Common
{
    public class ErrorViewModel
    {
        public Core.StatusCode Status { get; set; }
        public string Message { get; set; }

        public ErrorViewModel(Core.StatusCode status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
