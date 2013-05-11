using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridescent.Utils
{
    public sealed class ValueConverter
    {
        public static T Parse<T>(string s, T failedValue = default(T), bool rethrow = false) where T : IConvertible
        {
            T result = failedValue;
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
