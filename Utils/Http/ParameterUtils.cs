using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Iridescent.Utils.Http
{
    public static class ParameterUtils
    {
        public static string BuildQueryString(Dictionary<string, string> parameters)
        {
            IList<string> pairs = new List<string>();
            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;

                pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(item.Value)));
            }

            return string.Join("&", pairs.ToArray());
        }

        public static Dictionary<string,string> BuildDictionaryFromQueryString(string queryString)
        {
            var dict = new Dictionary<string, string>();
            var arr = queryString.Split('&');
            foreach (var item in arr)
            {
                var tmp = item.Split('=');
                dict.Add(tmp[0], tmp[1]);
            }

            return dict;
        }

        public static string BuildQueryString(params RequestParameter[] parameters)
        {
            return BuildQueryString(true, parameters);
        }

        public static string BuildQueryString(bool encode, params RequestParameter[] parameters)
        {
            IList<string> pairs = new List<string>();
            foreach (var item in parameters)
            {
                if (item.IsBinaryData)
                    continue;

                var value = string.Format("{0}", item.Value);
                if (!string.IsNullOrEmpty(value))
                {
                    string name=item.Name;
                    if(encode)
                    {
                        name = Uri.EscapeDataString(item.Name);
                        value = Uri.EscapeDataString(value);
                    }
                    pairs.Add(string.Format("{0}={1}", name, value));
                }
            }

            return string.Join("&", pairs.ToArray());
        }

        public static string GetBoundary()
        {
            string pattern = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder boundaryBuilder = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                var index = rnd.Next(pattern.Length);
                boundaryBuilder.Append(pattern[index]);
            }
            return boundaryBuilder.ToString();
        }

        /// <summary>
        /// 创建Post Body
        /// </summary>
        /// <param name="boundary"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static byte[] BuildPostData(string boundary, params RequestParameter[] parameters)
        {
            List<RequestParameter> pairs = new List<RequestParameter>(parameters);
            pairs.Sort(new RequestParameterComparer());
            MemoryStream buff = new MemoryStream();

            byte[] headerBuff = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}\r\n", boundary));
            byte[] footerBuff = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}--", boundary));

            foreach (RequestParameter p in pairs)
            {
                if (!p.IsBinaryData)
                {
                    var value = string.Format("{0}", p.Value);
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    buff.Write(headerBuff, 0, headerBuff.Length);
                    byte[] dispositonBuff = Encoding.UTF8.GetBytes(string.Format("content-disposition: form-data; name=\"{0}\"\r\n\r\n{1}", p.Name, p.Value.ToString()));
                    buff.Write(dispositonBuff, 0, dispositonBuff.Length);

                }
                else
                {
                    buff.Write(headerBuff, 0, headerBuff.Length);
                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"image/unknow\"\r\nContent-Transfer-Encoding: binary\r\n\r\n";
                    byte[] fileBuff = System.Text.Encoding.UTF8.GetBytes(string.Format(headerTemplate, p.Name, string.Format("upload{0}.jpg", BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0))));
                    buff.Write(fileBuff, 0, fileBuff.Length);
                    byte[] file = (byte[])p.Value;
                    buff.Write(file, 0, file.Length);

                }
            }

            buff.Write(footerBuff, 0, footerBuff.Length);
            buff.Position = 0;

            byte[] contentBuff = new byte[buff.Length];
            buff.Read(contentBuff, 0, contentBuff.Length);
            buff.Close();
            buff.Dispose();
            return contentBuff;
        }

    }
}
