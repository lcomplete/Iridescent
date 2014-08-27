using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Iridescent.Cache
{
    public class WebCache :ICache
    {

        public bool Set(string key, object value)
        {
            HttpRuntime.Cache.Insert(key,value);
            return true;
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            HttpRuntime.Cache.Insert(key, value, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);
            return true;
        }

        public bool Set(string key, object value, TimeSpan validateFor)
        {
            Set(key, value, DateTime.Now.Add(validateFor));
            return true;
        }

        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public T Get<T>(string key)
        {
            object obj = Get(key);
            return obj != null ? (T)obj : default(T);
        }

        public bool Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
            return true;
        }

        public void FlushAll()
        {
            IDictionaryEnumerator cacheEnumerator = HttpRuntime.Cache.GetEnumerator();
            do
            {
                HttpRuntime.Cache.Remove(cacheEnumerator.Key.ToString());
            } while (cacheEnumerator.MoveNext());
        }
    }
}
