using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Iridescent.Ajax
{
    public class AjaxHandlerFactory : IHttpHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            string fileWithoutExtensions = Path.GetFileNameWithoutExtension(url);
            if (Utility.UrlNamespaceMappings.ContainsKey(fileWithoutExtensions))
            {
                bool isPost = requestType == "POST";
                Type type = Type.GetType(fileWithoutExtensions, true, true);
                if (isPost)
                {
                    string method = context.Request.Headers["X-IridescentAjax-Method"];
                    MethodInfo methodInfo = type.GetMethod(method);
                    AjaxMethodAttribute[] attributes=(AjaxMethodAttribute[])methodInfo.GetCustomAttributes(typeof (AjaxMethodAttribute), true);
                    switch (attributes[0].SessionState)
                    {
                        case HttpSessionState.None:
                            return new AjaxProcessorHttpHandler(type);
                        case HttpSessionState.ReadWrite:
                            return new AjaxProcessorHttpHandlerSession(type);
                        case HttpSessionState.ReadOnly:
                            return new AjaxProcessorHttpHandlerSessionReadOnly(type);
                    }
                }
                return new TypeJavascriptHandler(type);
            }
            return new EmbeddedJavascriptHandler("Core.js");
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}
