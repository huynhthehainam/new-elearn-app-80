using eLearnApps.Core.Domain.Users;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eLearnApps.Core.Extension
{
    public static class Extensions
    {
        public static string FindRoleFromEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            var atidx = email.IndexOf("@");
            if (atidx <= -1 || atidx >= email.Length - 1)
                return string.Empty;

            var domain = email.Substring(atidx + 1);
            var dotidx = domain.IndexOf('.');
            if (dotidx <= 0 || dotidx >= domain.Length)
                return string.Empty;

            var roleByEmail = domain.Substring(0, dotidx);
            return roleByEmail;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).First();
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public static byte[] ToByteArray(this string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static IEnumerable<R> SelectAsync<T, R>(this IEnumerable<T> source, Func<T, Task<R>> selector)
        {
            foreach (var item in source)
                yield return selector(item).Result;
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> list, int nSize)
        {
            for (int i = 0; i < list.Count; i += nSize)
            {
                yield return list.GetRange(i, Math.Min(nSize, list.Count - i));
            }
        }

        public static IDictionary<string, object> ToDictionary(this NameValueCollection collection)
        {
            var dict = new Dictionary<string, object>();

            foreach (string? key in collection.AllKeys)
            {
                var value = collection[key];
                if (!string.IsNullOrEmpty(key) && value != null)
                    dict.Add(key, value);
            }

            return dict;
        }

        public static void AssignTo<TSource, TTarget>(this TSource source, TTarget target) where TSource : notnull where TTarget : notnull
        {
            if (EqualityComparer<TSource>.Default.Equals(source, default) ||
                EqualityComparer<TTarget>.Default.Equals(target, default))
                return;

            var typeSource = source.GetType();
            var typeTarget = target.GetType();
            foreach (var targetProperty in typeTarget.GetProperties())
            {
                var sourceProperty = typeSource.GetProperty(targetProperty.Name);
                if (sourceProperty == null)
                    continue;

                var sourcePropertyType = sourceProperty.PropertyType;
                var targetPropertyType = targetProperty.PropertyType;
                object? value = sourceProperty.GetValue(source);
                if (value == null)
                {
                    targetProperty.SetValue(target, GetDefaultValue(targetPropertyType));
                    continue;
                }
                var targetUnderlyingType = Nullable.GetUnderlyingType(targetPropertyType) ?? targetPropertyType;
                if (sourcePropertyType != targetPropertyType)
                {
                    try
                    {
                        value = Convert.ChangeType(value, targetUnderlyingType);
                    }
                    catch
                    {
                        continue;
                    }
                }
                targetProperty.SetValue(target, value);
            }
        }

        private static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
                return Activator.CreateInstance(type);
            else
                return null;
        }

        public static T ConvertTo<T>(this object source) where T : notnull, new()
        {
            var target = new T();
            source.AssignTo(target);
            return target;
        }

        public static IEnumerable<T> ConvertTo<T>(this IEnumerable<object> source) where T : new()
        {
            if (source == null || !source.Any())
                return Enumerable.Empty<T>();

            return source.Select(sourceElement => sourceElement.ConvertTo<T>());
        }

        /// <summary>
        /// Retrieves logged in user information from the HttpContext.
        /// </summary>
        public static LoggedInUserInfo GetLoggedInUserInfo(this HttpContext httpContext)
        {
            var userInfo = new LoggedInUserInfo();
            var user = httpContext.User;
            if (httpContext == null || user == null)
                return userInfo;

            var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                if (int.TryParse(userId, out int id))
                    userInfo.UserId = id;
            }

            var claims = user.Claims.ToList();
            if (claims != null && claims.Any())
            {
                var courseClaim = claims.FirstOrDefault(x =>
                    x.Type.Equals(Constants.CourseIdClaim, StringComparison.OrdinalIgnoreCase));
                if (courseClaim != null && int.TryParse(courseClaim.Value, out int courseId))
                {
                    userInfo.CourseId = courseId;
                }
                userInfo.ToolName = claims.FirstOrDefault(x =>
                    x.Type.Equals(Constants.ToolNameClaim, StringComparison.OrdinalIgnoreCase))?.Value;
                userInfo.RoleName = claims.FirstOrDefault(x =>
                    x.Type.Equals(Constants.RoleClaim, StringComparison.OrdinalIgnoreCase))?.Value;
            }
            return userInfo;
        }

        public static string ToJson<T>(this T input)
        {
            if (input == null)
                return string.Empty;

            var json = JsonSerializer.Serialize(input);
            return json;
        }

        public static string ToIsoFormat(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }
}
