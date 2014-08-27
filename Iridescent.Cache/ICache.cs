using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridescent.Cache
{
    public interface ICache
    {
        bool Set(string key, object value);
        bool Set(string key, object value, DateTime expiresAt);
        bool Set(string key, object value, TimeSpan validateFor);

        object Get(string key);
        T Get<T>(string key);

        bool Remove(string key);

        void FlushAll();
    }
}
