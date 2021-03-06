﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Iridescent.Cache;
using ServiceStack.Redis;

namespace Iridescent.Redis
{
    /// <summary>
    /// Redis缓存类
    /// </summary>
    public class RedisCache:ICache
    {
        /// <summary>
        /// 将缓存值包装为强类型
        /// </summary>
        [Serializable]
        class ValueWrapper
        {
            public object Value { get; private set; }

            public ValueWrapper(object value)
            {
                Value = value;
            }
        }

        public bool Set(string key, object value)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, new ValueWrapper(value));
            }
        }

        public bool Set(string key, object value, DateTime expiresAt)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, new ValueWrapper(value), expiresAt);
            }
        }

        public bool Set(string key, object value, TimeSpan expiresIn)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                return client.Set(key, new ValueWrapper(value), expiresIn);
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
                ValueWrapper wrapper= client.Get<ValueWrapper>(key);
                if (wrapper != null)
                {
                    return (T) wrapper.Value;
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