using Microsoft.Extensions.Configuration;

namespace eLearnApps.Valence
{
    public class Constants
    {
        private readonly IConfiguration _configuration;
        public Constants(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // this is for old 
        public int CourseIndicator = 3;
        public int DefaultHttpsPost = 443;
        public string Https = "https";
        public string BasePath = "/d2l/api/lp/";
        public string LEBasePath = "/d2l/api/le/";
        public string CustomizationBasePath = "/d2l/api/customization/1.0/";

        public string BaseHost => $"{Https}://{LsmHost}";
        public string ValenceApiAppId => _configuration.GetValue<string>("ValenceApi:AppId") ?? "";
        public string ValenceApiAppKey => _configuration.GetValue<string>("ValenceApi:AppKey") ?? "";
        public string LsmHost => _configuration.GetValue<string>("ValenceApi:LsmHost") ?? "";
        public string ValenceApiUserId => _configuration.GetValue<string>("ValenceApi:UserId") ?? "";
        public string ValenceApiUserKey => _configuration.GetValue<string>("ValenceApi:UserKey") ?? "";

        public string LpVersion
        {
            get
            {
                var apiversion = _configuration.GetValue<string>("ValenceApi:LPVersion");
                if (apiversion == null)
                {
                    // default api version
                    apiversion = "1.27";
                }

                return apiversion;
            }
        }

        public string ValenceLEVersion
        {
            get
            {
                var apiversion = _configuration.GetValue<string>("ValenceApi:LEVersion");
                if (apiversion == null)
                {
                    // default api version
                    apiversion = "1.26";
                }

                return apiversion;
            }
        }
    }
}