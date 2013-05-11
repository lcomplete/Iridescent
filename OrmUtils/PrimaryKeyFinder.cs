using System;
using System.Reflection;

namespace Iridescent.OrmExpress
{
    public class PrimaryKeyFinder
    {
        private const string DefaultPrimaryKey = "Id";

        public static string GetPrimaryKey<TEntity>()
        {
            return GetPrimaryKey(typeof (TEntity));
        }

        public static string GetPrimaryKey(Type type)
        {
            PropertyInfo primaryKeyProperty = GetPrimaryKeyPropertyInfo(type);
            return primaryKeyProperty.Name;
        }

        public static PropertyInfo GetPrimaryKeyPropertyInfo(Type type)
        {
            PropertyInfo primaryKeyProperty = ReflectHelper.SearchPropertyInfoFromAttribute<PrimaryKeyAttribute>(type);
            if (primaryKeyProperty == null)
                primaryKeyProperty = type.GetProperty(DefaultPrimaryKey, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public|BindingFlags.IgnoreCase);
            if (primaryKeyProperty == null)
                throw new ArgumentException("无法找到主键属性");
            return primaryKeyProperty;
        }

    }
}