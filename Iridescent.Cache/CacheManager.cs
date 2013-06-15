using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridescent.Cache
{
    public class CacheManager : ICache
    {
        private ICache _cacheProvider;

        private string _cacheGroup;
        public string CacheGroup
        {
            get { return _cacheGroup; }
        }

        public CacheManager(string cacheGroup = "")
        {
            _cacheGroup = cacheGroup;
            _cacheProvider = CacheFactory.Create();
        }

        public T Get<T>(string key, Func<T> ifnotfound, TimeSpan cacheTime) where T : class
        {
            T obj = Get<T>(key);
            if (obj == default(T))
            {
                obj = ifnotfound.Invoke();
                if (obj != null)
                    Set(key, obj, cacheTime);
            }

            return obj;
        }

        #region CacheProvider 委托

        public bool Set(string key, object value)
        {
            return _cacheProvider.Set(CacheGroup + key, value);
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            return _cacheProvider.Set(CacheGroup + key, value, expiresAt);
        }

        public bool Set(string key, object value, TimeSpan validateFor)
        {
            return _cacheProvider.Set(CacheGroup + key, value, validateFor);
        }

        public object Get(string key)
        {
            return _cacheProvider.Get(CacheGroup + key);
        }

        public T Get<T>(string key) where T : class
        {
            return _cacheProvider.Get<T>(CacheGroup + key);
        }

        public bool Remove(string key)
        {
            return _cacheProvider.Remove(CacheGroup + key);
        }

        public void FlushAll()
        {
            _cacheProvider.FlushAll();
        }

        #endregion
    }
}
