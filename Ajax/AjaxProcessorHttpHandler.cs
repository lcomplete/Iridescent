using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Iridescent.Ajax
{
    internal class AjaxProcessorHttpHandler:IHttpHandler
    {
        private Type _type;

        public AjaxProcessorHttpHandler(Type type)
        {
            _type = type;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string method = context.Request.Headers["X-IridescentAjax-Method"];
            MethodInfo methodInfo = _type.GetMethod(method);
            context.Response.Write(InvokeMethod(context,methodInfo).ToString());
        }

        private object InvokeMethod(HttpContext context,MethodInfo methodInfo)
        {
            if(methodInfo!=null)
            {
                var parameters = methodInfo.GetParameters();
                object[] methodParameters=new object[parameters.Length];
                for (int i=0;i<parameters.Length;i++)
                {
                    var parameterInfo = parameters[i];
                    Type parameterType = parameterInfo.ParameterType;
                    if(!parameterType.IsValueType && parameterType!=typeof(string))
                    {
                        throw new Exception("只能处理参数为值类型和字符串的ajax方法");
                    }
                    string postParameter = context.Request.Form[parameterInfo.Name];
                    if(string.IsNullOrEmpty(postParameter))
                    {
                        methodParameters[i] = ReflectionHelper.GetDefaultValue(parameterType);
                    }
                    else
                    {
                        methodParameters[i] = ReflectionHelper.ToType(parameterType, postParameter);
                    }
                }
                object obj = null;
                if (!methodInfo.IsStatic)
                    obj = Activator.CreateInstance(_type);
                return methodInfo.Invoke(obj, methodParameters);
            }
            return null;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
