using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridescent.Cache;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Iridescent.Redis
{
    /// <summary>
    /// 弱类型的redis缓存类(RedisCache 已进行强类型封装，该类弃用)
    /// </summary>
    [Obsolete]
    public class WakeTypingRedisCache:ICache
    {
        public bool Set(string key, object value)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, WrapCacheValue(value));
            }
        }

        private KeyValuePair<Type, object> WrapCacheValue(object cacheValue)
        {
            return new KeyValuePair<Type, object>(cacheValue.GetType(), cacheValue);
        } 

        public bool Set(string key, object value, DateTime expiresAt)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, WrapCacheValue(value), expiresAt);
            }
        }

        public bool Set(string key, object value, TimeSpan expiresIn)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, WrapCacheValue(value), expiresIn);
            }
        }

        public object Get(string key)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return null;

            object result = null;
            using (client)
            {
                var cacheResult = client.Get<KeyValuePair<Type, string>>(key);
                if (cacheResult.Key != null)
                {
                    result = JsonSerializer.DeserializeFromString(cacheResult.Value, cacheResult.Key);
                }
            }

            return result;
        }

        public T Get<T>(string key)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return default(T);

            T result = default(T);
            using (client)
            {
                var cacheResult = client.Get<KeyValuePair<Type, T>>(key);
                if (cacheResult.Key != null)
                {
                    result = cacheResult.Value;
                }
            }

            return result;
        }

        public bool Remove(string key)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Remove(key);
            }
        }

        public void FlushAll()
        {
            throw new NotSupportedException("缓存为分布式结构，暂不支持清空所有缓存");
        }   
    }
}
