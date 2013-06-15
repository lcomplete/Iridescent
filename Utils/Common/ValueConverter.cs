using System;

namespace Iridescent.Utils.Common
{
    public static class ValueConverter
    {
        public static T Parse<T>(string s, T failedValue = default(T), bool rethrow = false) where T : IConvertible
        {
            T result = failedValue;
            if(string.IsNullOrWhiteSpace(s) && !rethrow)
                return result;

            try
            {
                var convertible = (IConvertible)s;
                object typeObj = convertible.ToType(typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                result = (T)typeObj;
            }
            catch
            {
                if (rethrow)
                {
                    throw;
                }
            }
            return result;
        }
    }
}
