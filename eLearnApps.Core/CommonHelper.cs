using System;
using System.IO;

namespace eLearnApps.Core
{
    public static class CommonHelper
    {
        /// <summary>
        ///     Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <param name="contentRootPath">Injected content root path (e.g., from IWebHostEnvironment)</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path, string contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(path))
                return contentRootPath;

            path = path.Replace("~/", "").TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(contentRootPath, path);
        }
    }
}
