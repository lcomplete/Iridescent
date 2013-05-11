using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Iridescent.Ajax
{
    public sealed class ReflectionHelper
    {
        ReflectionHelper(){}

        public static object ToType(Type type,string value)
        {
            if (type == typeof(string))
                return value;

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            MethodInfo parseMethod = (from method in methods
                                     where method.Name == "Parse" && method.GetParameters().Length == 1
                                     select method).FirstOrDefault();
            if(parseMethod==null)
            {
                throw new ArgumentException(string.Format("Type: {0} has not Parse static method!",type.ToString()));
            }
            return parseMethod.Invoke(null, new object[] { value });
        }

        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
