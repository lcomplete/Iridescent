using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Iridescent.Ajax
{
    internal class EmbeddedJavascriptHandler:IHttpHandler
    {
        private const string FILE_RESOURCE_PREFIX = "Iridescent.Ajax.Script.";
        private string _fileName;

        public EmbeddedJavascriptHandler(string fileName)
        {
            _fileName = fileName;
        }

        public void ProcessRequest(HttpContext context)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string fullFilename = FILE_RESOURCE_PREFIX + _fileName;
            Stream resourceStream = assembly.GetManifestResourceStream(fullFilename);

            if (resourceStream != null)
            using (StreamReader reader=new StreamReader(resourceStream))
            {
                context.Response.AddHeader("Content-Type", "application/x-javascript");
                context.Response.Write(reader.ReadToEnd());
            }
            else
            {
                throw new Exception("名为 "+fullFilename+" 的资源不存在。");
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
