using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Iridescent.Ajax
{
    internal class TypeJavascriptHandler:IHttpHandler
    {
        private Type _type;

        public TypeJavascriptHandler(Type type)
        {
            _type = type;
        }

        public void ProcessRequest(HttpContext context)
        {
            MethodInfo[] methodInfos = _type.GetMethods();
            MethodInfo[] ajaxMethodInfos = (from methodInfo in methodInfos
                                            where
                                                methodInfo.GetCustomAttributes(typeof (AjaxMethodAttribute), true).
                                                    Length != 0
                                            select methodInfo).ToArray();
            if (ajaxMethodInfos.Length == 0)
            {
                context.Response.Write("// the type doesn't have ajaxmethod.");
                return;
            }
            context.Response.AddHeader("Content-Type", "application/x-javascript");
            context.Response.Write(GetTypeJavaScript(ajaxMethodInfos));
        }

        private string GetTypeJavaScript(MethodInfo[] ajaxMethodInfos)
        {
            StringBuilder javascriptBuilder = new StringBuilder();
            javascriptBuilder.Append("// author: lcomplete,\n\n");
            javascriptBuilder.AppendFormat("if(typeof {0} ==\"undefined\") {0}={{}};\n", _type.Namespace);
            javascriptBuilder.AppendFormat("if(typeof {0}_class ==\"undefined\") {0}_class={{}};\n", _type.FullName);
            javascriptBuilder.AppendFormat("{0}_class=function(){{}};\n", _type.FullName);
            javascriptBuilder.AppendFormat(
                "Object.extend({0}_class.prototype,Object.extend(new Iridescent.AjaxClass(), {{\n", _type.FullName);
            foreach (var ajaxMethodInfo in ajaxMethodInfos)
            {
                var parameters = ajaxMethodInfo.GetParameters();
                var joinParameters = (from parameterInfo in parameters
                                      select parameterInfo.Name).ToArray();
                javascriptBuilder.AppendFormat("\t{0}:function({1}) {{\n", ajaxMethodInfo.Name,
                                               string.Join(",", joinParameters));
                javascriptBuilder.AppendFormat("\t\treturn this.invoke(\"{0}\",{{{1}}},this.{0}.getArguments().slice({2}));\n",
                    ajaxMethodInfo.Name,
                    string.Join(",",(from joinParameter in joinParameters 
                         select "\""+joinParameter+"\":"+joinParameter).ToArray()),
                     parameters.Length);
                javascriptBuilder.Append("\t},\n");
            }
            javascriptBuilder.AppendFormat(
                "\turl:\"/Iridescent/Ajax/{0}.ashx\"\n}}));\n", _type.FullName + "," + _type.Assembly.GetName().Name);
            javascriptBuilder.AppendFormat("{0}=new {0}_class();", _type.FullName);

            return javascriptBuilder.ToString();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
