using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{

    public class GradeResultViewModel
    {
        public List<GradeResultModel> GradeResults { set; get; }
        public GradeInfoModel GradeInfo { set; get; }
        public string Message { set; get; }
        public bool ShowMessage { get; set; }
        public bool QualifiedAsTutor { get; set; }
        public bool RegisteredAsTutor { get; set; }
        public string RecommendedBy { get; set; }
        public string UserKey { get; set; }
        public string PeerTutoringSystemURL { get; set; }
    }
}
