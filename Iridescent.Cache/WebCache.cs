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
        private HttpContext _context;

        public WebCache()
        {
            _context = HttpContext.Current;
            if(_context==null)
            {
                throw new HttpRequestValidationException("不存在http请求上下文");
            }
        }

        public bool Set(string key, object value)
        {
            _context.Cache.Insert(key,value);
            return true;
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            _context.Cache.Insert(key, value, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);
            return true;
        }

        public bool Set(string key, object value, TimeSpan validateFor)
        {
            Set(key, value, DateTime.Now.Add(validateFor));
            return true;
        }

        public object Get(string key)
        {
            return _context.Cache.Get(key);
        }

        public T Get<T>(string key) where T:class 
        {
            return Get(key) as T;
        }

        public bool Remove(string key)
        {
            _context.Cache.Remove(key);
            return true;
        }

        public void FlushAll()
        {
            IDictionaryEnumerator cacheEnumerator = _context.Cache.GetEnumerator();
            do
            {
                _context.Cache.Remove(cacheEnumerator.Key.ToString());
            } while (cacheEnumerator.MoveNext());
        }
    }
}
