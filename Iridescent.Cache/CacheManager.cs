using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridescent.Cache
{
    public class CacheManager : ICache
    {
        private ICache _cacheProvider;

        private string _keyPrefix;
        public string KeyPrefix
        {
            get { return _keyPrefix; }
        }

        public CacheManager(string keyPrefix = "")
        {
            _keyPrefix = keyPrefix;
            _cacheProvider = CacheFactory.Create();
        }



        #region ICache 接口成员
        
        public bool Set(string key, object value)
        {
            return _cacheProvider.Set(KeyPrefix + key, value);
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            return _cacheProvider.Set(KeyPrefix + key, value, expiresAt);
        }

        public bool Set(string key, object value, TimeSpan validateFor)
        {
            return _cacheProvider.Set(KeyPrefix + key, value, validateFor);
        }

        public object Get(string key)
        {
            return _cacheProvider.Get(KeyPrefix + key);
        }

        public T Get<T>(string key) where T : class
        {
            return _cacheProvider.Get<T>(KeyPrefix + key);
        }

        public bool Remove(string key)
        {
            return _cacheProvider.Remove(KeyPrefix + key);
        }

        public void FlushAll()
        {
            _cacheProvider.FlushAll();
        }

        #endregion
    }
}
