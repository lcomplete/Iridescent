using System;
using System.Text.RegularExpressions;

namespace Iridescent.Utils.Verify
{
    public static partial class VerifyUtils
    {
        public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string UrlRegex = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";

        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, EmailRegex);
        }

        public static bool IsUrl(string url)
        {
            return Regex.IsMatch(url, UrlRegex);
        }

        public static bool IsIpAddress(string ipAddress)
        {
            return Regex.IsMatch(ipAddress, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsMobile(string mobile)
        {
            return Regex.IsMatch(mobile, @"^(15|13|18)\d{9}$");
        }

        /// <summary>
        /// 判断字符是否仅包含字母或数字或字母和数字
        /// </summary>
        /// <returns></returns>
        public static bool IsLetterOrNumber(string str)
        {
            return Regex.IsMatch(str, @"^([a-z]|\d)+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判断字符是否在指定长度范围（闭区间）内
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static bool IsInLength(string str, int lowerBound, int upperBound)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return str.Length <= upperBound && str.Length >= lowerBound;
        }

        /// <summary>
        /// 是否为数字
        /// </summary>
        /// <param name="numStr"></param>
        /// <returns></returns>
        public static bool IsInt(string numStr)
        {
            if (string.IsNullOrWhiteSpace(numStr))
                return false;
            int outi;
            return int.TryParse(numStr, out outi);
        }

        public static bool IsFloat(string numStr)
        {
            if (string.IsNullOrWhiteSpace(numStr))
                return false;
            float outi;
            return float.TryParse(numStr, out outi);
        }

        public static bool IsDouble(string numStr)
        {
            if (string.IsNullOrWhiteSpace(numStr))
                return false;
            double outi;
            return double.TryParse(numStr, out outi);
        }

        public static bool IsDatetime(string numStr)
        {
            if (string.IsNullOrWhiteSpace(numStr))
                return false;
            DateTime outi;
            return DateTime.TryParse(numStr, out outi);
        }


        public static string FilterStr(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return str.Replace("'", "''").Trim();
        }
    }
}
