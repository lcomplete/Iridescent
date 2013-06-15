using System;
using System.Net;

namespace Iridescent.Utils.Common
{
    public static class UrlUtils
    {
        /// <summary>
        /// 是否是域名
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public static bool IsDomain(string siteUrl)
        {
            try
            {
                if (!siteUrl.StartsWith("http://") && !siteUrl.StartsWith("https://"))
                {
                    siteUrl = "http://" + siteUrl;
                }
                Uri uri = new Uri(siteUrl);
                return uri.PathAndQuery=="/";
            }
            catch
            {
                return false;
            }
        }

    }
}
