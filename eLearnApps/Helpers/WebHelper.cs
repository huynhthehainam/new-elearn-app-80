

using Microsoft.Extensions.Primitives;

namespace eLearnApps.Helpers
{
    public class WebHelper
    {

        private HttpContext? _httpContext;

        public WebHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }
        public WebHelper(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUrlReferrer()
        {
            return _httpContext?.Request.Headers["Referer"].ToString() ?? string.Empty;
        }

        public string GetReferrerControllerName()
        {
            var fullUrl = GetUrlReferrer();
            return GetControllerName(fullUrl);
        }

        public string GetControllerName(string fullUrl)
        {
            if (string.IsNullOrWhiteSpace(fullUrl)) return "unspecified";

            try
            {
                var routeData = _httpContext?.GetRouteData();
                return routeData?.Values["controller"]?.ToString() ?? "unspecified";
            }
            catch
            {
                return "unspecified";
            }
        }

        public string GetCurrentIpAddress()
        {
            if (_httpContext == null) return string.Empty;

            try
            {
                if (_httpContext.Request.Headers.TryGetValue("X-FORWARDED-FOR", out StringValues forwardedFor))
                {
                    var ip = forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault();
                    if (!string.IsNullOrEmpty(ip)) return ip;
                }
                return _httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetThisPageUrl(bool includeQueryString)
        {
            return GetThisPageUrl(includeQueryString, IsCurrentConnectionSecured());
        }

        public string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            if (_httpContext == null) return string.Empty;

            var request = _httpContext.Request;
            var scheme = useSsl ? "https" : "http";
            var host = request.Host.Value;
            var path = request.Path;
            var queryString = includeQueryString ? request.QueryString.ToString() : string.Empty;

            return $"{scheme}://{host}{path}{queryString}".ToLowerInvariant();
        }

        public bool IsCurrentConnectionSecured()
        {
            return _httpContext?.Request.IsHttps ?? false;
        }

        public string ServerVariables(string name)
        {
            if (_httpContext?.Request.Headers.TryGetValue(name, out StringValues value) == true)
                return value.ToString();

            return string.Empty;
        }

        public string CategoryName => GetCategoryName(GetReferrerControllerName());

        public string GetCategoryNameFromUrl(string url)
        {
            return GetCategoryName(GetControllerName(url));
        }

        private string GetCategoryName(string controllerName)
        {
            if (string.Equals(FunctionName.Ffts.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.Ffts.ToString();
            if (string.Equals(FunctionName.InClassSensing.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return FunctionName.InClassSensing.ToString();
            if (string.Equals(FunctionName.Cmt.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.Cmt.ToString();
            if (string.Equals(FunctionName.Ee.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.Ee.ToString();
            if (string.Equals(FunctionName.Pet.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.Pet.ToString();
            if (string.Equals(FunctionName.RPT.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.Rpt.ToString();
            if (string.Equals(FunctionName.Journal.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.Journal.ToString();
            if (string.Equals(FunctionName.Security.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.System.ToString();
            if (string.Equals(FunctionName.PeerFeedback.ToString(), controllerName, StringComparison.OrdinalIgnoreCase))
                return Category.PeerFeedback.ToString();
            return Category.Ffts.ToString();
        }
    }
}