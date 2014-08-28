using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Iridescent.Utils.Http
{
    internal class RequestUtils
    {
        /// <summary>
        /// 发起Http请求，对参数进行URL编码
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>请求结果</returns>
        /// <exception cref="BadRequestException">请求返回错误结果引发的异常</exception>
        public static string Request(string url, RequestMethod method = RequestMethod.Get, params RequestParameter[] parameters)
        {
            return Request(url, null, true, method, parameters);
        }

        /// <summary>
        /// 发起Http请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headerAuthorization">授权验证头部</param>
        /// <param name="encode">是否对参数进行编码</param>
        /// <param name="method">请求方式</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>请求结果</returns>
        /// <exception cref="BadRequestException">请求返回错误结果引发的异常</exception>
        public static string Request(string url, string headerAuthorization, bool encode,RequestMethod method = RequestMethod.Get, params RequestParameter[] parameters)
        {
            string result = string.Empty;
            UriBuilder uri = new UriBuilder(url);

            if (method == RequestMethod.Get)
            {
                if (!string.IsNullOrEmpty(uri.Query))
                    uri.Query = uri.Query.TrimStart('?') + "&" + ParameterUtils.BuildQueryString(encode, parameters);
                else
                    uri.Query = ParameterUtils.BuildQueryString(encode, parameters);
            }

            HttpWebRequest http = WebRequest.Create(uri.Uri) as HttpWebRequest;

            if (!string.IsNullOrEmpty(headerAuthorization))
            {
                http.Headers.Add("Authorization:" + headerAuthorization);
            }

            switch (method)
            {
                case RequestMethod.Get:
                    {
                        http.Method = "GET";
                    }
                    break;
                case RequestMethod.Post:
                    {
                        http.Method = "POST";
                        bool multi = parameters.Count(p => p.IsBinaryData) > 0;

                        if (multi)
                        {
                            string boundary = ParameterUtils.GetBoundary();
                            http.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                            http.AllowWriteStreamBuffering = true;
                            using (Stream request = http.GetRequestStream())
                            {
                                var raw = ParameterUtils.BuildPostData(boundary, parameters);
                                request.Write(raw, 0, raw.Length);
                            }
                        }
                        else
                        {
                            http.ContentType = "application/x-www-form-urlencoded";

                            using (StreamWriter request = new StreamWriter(http.GetRequestStream()))
                            {
                                request.Write(ParameterUtils.BuildQueryString(encode,parameters));
                            }
                        }
                    }
                    break;
            }

            try
            {
                using (WebResponse response = http.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException webEx)
            {
                string response = string.Empty;
                if (webEx.Response != null)
                {
                    using (StreamReader reader = new StreamReader(webEx.Response.GetResponseStream()))
                    {
                        response = reader.ReadToEnd();
                    }
                }
                
                throw new BadRequestException(webEx.Message,response,webEx);
            }

            return result;
        }

    }
}
