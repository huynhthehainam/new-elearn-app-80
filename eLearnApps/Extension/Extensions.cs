using eLearnApps.Core.Cryptography;
using eLearnApps.Entity.Security;
using eLearnApps.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace eLearnApps.Extension
{
    public class Extensions
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ClaimHelper _claimHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Extensions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _claimHelper = new ClaimHelper(_serviceProvider);
        }
        public string ContentAbsUrl(UrlHelper url, string relativeContentPath)
        {
            //var contextUri = HttpContext.Current.Request.Url;

            //var baseUri = string.Format("{0}://{1}", contextUri.Scheme,
            //    contextUri.Host);

            //return string.Format("{0}{1}", baseUri, VirtualPathUtility.ToAbsolute(relativeContentPath));
            var request = _httpContextAccessor.HttpContext.Request;

            var baseUri = $"{request.Scheme}://{request.Host}";

            return $"{baseUri}{url.Content(relativeContentPath)}";
        }

        public SelectList ToSelectList<TEnum>(TEnum enumObj,
            bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true)
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");
            return null;
        }

        public string GetSpecificClaim(ClaimsIdentity claimsIdentity, string claimType)
        {
            var courseId = _httpContextAccessor.HttpContext.Request.Query["courseId"].ToString();

            var claim = claimsIdentity.Claims.FirstOrDefault(x =>
                x.Type == nameof(StandardPermissionProvider) && $"{courseId}_{claimType}" == x.Value);

            return claim != null ? claim.Value : string.Empty;
        }

        public bool HasClaim(IIdentity identity, string claimType)
        {
            var courseId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Query["courseId"].ToString());
            var userIdStr = (identity as ClaimsIdentity)?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return false;

            var enrollment = _claimHelper.GetEnrollmentFromCache(userId, courseId);

            if (enrollment != null)
            {
                var roleId = enrollment.RoleId;
                var permissions = _claimHelper.GetPermissionCache(roleId);
                var havePermission = permissions.Where(p =>
                    string.Equals(p.SystemName, claimType, StringComparison.InvariantCultureIgnoreCase)).Any();
                if (havePermission) return true;
            }

            return false;
        }

        public bool IsLoggedInUserA(IIdentity identity, string role)
        {
            var courseId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Query["courseId"].ToString());
            var userIdStr = (identity as ClaimsIdentity)?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return false;
            var enrollment = _claimHelper.GetEnrollmentFromCache(userId, courseId);

            if (enrollment != null)
                return enrollment.RoleName.StartsWith(role) ? true : false;

            return false;
        }

        public static bool CheckClaim(ClaimsIdentity claimsIdentity, string courseId, string systemName)
        {
            var hasClaim = false;
            // TODO DRY (got repeat in claim requirement)
            var claimType = $"course_{courseId}";
            var enrollmentForCourse = claimsIdentity.Claims.Where(c => c.Type == claimType).ToList();
            if (enrollmentForCourse.Any())
            {
                var roleId = enrollmentForCourse.First().Value;
                claimType = $"role_{roleId}";
                hasClaim = claimsIdentity.HasClaim(claimType, systemName);
            }

            return hasClaim;
        }

        public string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public bool IsAjaxOrFetchRequest(HttpRequest request)
        {
            if (request?.Headers != null && request.Headers.TryGetValue("X-Is-Ajax-Request", out var ajaxHeader))
            {
                if (bool.TryParse(ajaxHeader, out var result))
                    return result;
            }

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public double Round(double value, int places)
        {
            var decimalPoints = Math.Abs(value - Math.Floor(value));
            var powerOfTen = Math.Pow(10, places);
            if (decimalPoints * powerOfTen >= 0.5)
                return Math.Ceiling(value * powerOfTen) / powerOfTen;
            return Math.Floor(value * powerOfTen) / powerOfTen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ToEncrypt<T>(T rawText, string key = "")
        {
            if (rawText == null) return string.Empty;

            return AesEncrypt.Encrypt(rawText.ToString(), key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encryptText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ToDecrypt<T>(T encryptText, string key = "")
        {
            if (encryptText == null) return string.Empty;

            return AesEncrypt.Decrypt(encryptText.ToString(), key);
        }

        public string CsvEscape(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            data = data.Replace(',', ' ');
            data = Regex.Replace(data, @"\t|\n|\r", " ");
            return data;
        }

        public string ImageFullPathToBase64(string path)
        {
            using var image = new Bitmap(path);
            using var destStream = new MemoryStream();

            // Save image as-is, no resizing or compression customization here
            image.Save(destStream, ImageFormat.Jpeg);

            var destBinary = destStream.ToArray();
            var base64String = Convert.ToBase64String(destBinary);
            return base64String;
        }
    }
}
