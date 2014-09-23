using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Iridescent.Cache;

namespace Iridescent.Memcached
{
    public class MemCache:ICache
    {
        private IMemcachedClient _client;

        public MemCache()
        {
            _client = MemcachedClientFactory.Create();
        }

        public bool Set(string key, object value)
        {
            return _client.Store(StoreMode.Set, key, value);
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            return _client.Store(StoreMode.Set, key, value, expiresAt);
        }

        public bool Set(string key, object value, TimeSpan expiresIn)
        {
            return _client.Store(StoreMode.Set, key, value, expiresIn);
        }

        public object Get(string key)
        {
            return _client.Get(key);
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }

        public bool Remove(string key)
        {
            return _client.Remove(key);
        }

        public void FlushAll()
        {
            _client.FlushAll();
        }
    }
}
