using System;
using Iridescent.Cache;
using ServiceStack.Redis;

namespace Iridescent.Redis
{
    /// <summary>
    /// 功能：Redis缓存类
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public class RedisCache:ICache
    {
        public bool Set(string key, object value)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, value);
            }
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, value, expiresAt);
            }
        }

        public bool Set(string key, object value, TimeSpan validateFor)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, value, validateFor);
            }
        }

        public object Get(string key)
        {
            throw new NotSupportedException("获取Redis缓存对象 必须指定类型");
        }

        public T Get<T>(string key)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return default(T);

            using (client)
            {
                return client.Get<T>(key);
            }
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