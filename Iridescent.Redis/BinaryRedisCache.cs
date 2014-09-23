using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Iridescent.Cache;
using ServiceStack.Redis;

namespace Iridescent.Redis
{
    public class BinaryRedisCache : ICache
    {

        public bool Set(string key, object value)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, BinarySerialize(value));
            }
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, BinarySerialize(value), expiresAt);
            }
        }

        private byte[] BinarySerialize(object value)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        public bool Set(string key, object value, TimeSpan expiresIn)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, BinarySerialize(value), expiresIn);
            }
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return default(T);

            T result = default(T);
            using (client)
            {
                byte[] cacheBuffer = client.Get<byte[]>(key);
                if (cacheBuffer != null)
                {
                    using (MemoryStream ms = new MemoryStream(cacheBuffer))
                    {
                        object obj = new BinaryFormatter().Deserialize(ms);
                        result = (T)obj;
                    }
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
