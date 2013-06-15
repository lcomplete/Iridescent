using System;
using System.Linq;
using System.Reflection;

namespace Iridescent.Utils.Common
{
    public sealed class ReflectHelper
    {
        public static string[] GetPropertyNames(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            return propertyInfos.Select(property => property.Name).ToArray();
        }

        public static Object GetPropertyValue(Object entity,string propertyName)
        {
            PropertyInfo property= entity.GetType().GetProperty(propertyName);
            return property.GetValue(entity, null);
        }

        /// <summary>
        /// 获取类型上含有指定attribute的属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo SearchPropertyInfoFromAttribute<TAttribute>(Type type) where TAttribute:Attribute 
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                object[] attributes = propertyInfo.GetCustomAttributes(typeof (TAttribute), true);
                if (attributes.Length == 1)
                    return propertyInfo;
            }
            return null;
        }

        public static void SetPropertyValue(object entity, string propertyName, object value)
        {
            Type type = entity.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
            propertyInfo.SetValue(entity,value,null);
        }
    }
}
