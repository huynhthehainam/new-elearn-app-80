using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;

namespace eLearnApps.Core
{
    public static class Util
    {
        public static string RandomColor(int id)
        {
            var rand = new Random();
            var c = Color.FromArgb(rand.Next(id),
                rand.Next(id),
                rand.Next(id));
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /// <summary>
        /// Count the number of "day" between start and end date
        /// </summary>
        /// <param name="day">Day of the week of interest</param>
        /// <param name="start">date to start counting</param>
        /// <param name="end">date to end counting</param>
        /// <returns></returns>
        public static int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            start = start.Date.AddDays((7 + day - start.DayOfWeek) % 7);
            if (end < start)
                return 0;
            else
                return ((int)(end - start).TotalDays) / 7 + 1;
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        public static string GetDescription<T>(this T enumerationValue)
        where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString() ?? "");
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString() ?? "";
        }

        public static string GetPhotoFileName(string campusId, int userId)
        {
            var filename = string.Empty;
            if (!string.IsNullOrWhiteSpace(campusId) && campusId.Length > 2)
            {
                //remove prefix and suffix
                filename = campusId.Remove(0, 1);
                filename = filename.Remove(filename.Length - 1, 1);
            }
            filename += Right(eLearnApps.Core.Constants.EightZeroPadding + userId, 8);
            return filename;
        }

        public static string Right(string strText, int intLength)
        {
            var strReturn = string.Empty;
            if (!string.IsNullOrWhiteSpace(strText) && strText.Length > intLength)
                strReturn = strText.Substring(strText.Length - intLength, intLength);
            return strReturn;
        }
        public static string FitDatabaseColumnLength(this string strText, int intLength)
        {
            if (string.IsNullOrEmpty(strText))
                return string.Empty;

            Encoding utf8 = Encoding.UTF8;
            var count = utf8.GetByteCount(strText);
            if (count > intLength)
                return strText.Substring(0, intLength);
            return strText;
        }
        public static double CalculateAttendancePercentage(double presentCount, double partialCount, double totalSession)
        {
            double dblTotalPercentage = 0;
            //Count Present Percentage
            if (totalSession > 0)
            {
                //Calculate for present
                dblTotalPercentage = (Convert.ToDouble(presentCount) + Convert.ToDouble(partialCount)) / Convert.ToDouble(totalSession) *
                                     Convert.ToDouble(100);

            }

            //return the value back
            return dblTotalPercentage;
        }
        public static string ToCsvString(this string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
    }
}