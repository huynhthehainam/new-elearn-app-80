using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class ProductVersion
    {
        public string ProductCode { get; set; }
        public string LatestVersion { get; set; }
        public List<string> SupportedVersions { get; set; }
    }
}
