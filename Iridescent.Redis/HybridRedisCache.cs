using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Iridescent.Cache;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Iridescent.Redis
{
    /// <summary>
    /// 混合缓存二进制和json序列化的数据
    /// </summary>
    public class HybridRedisCache:ICache
    {
        /// <summary>
        /// 将缓存值包装为强类型
        /// </summary>
        class ValueWrapper
        {
            public object Value { get; private set; }

            public Type ValueType { get; set; }

            public ValueWrapper(object value)
            {
                Value = value;
                ValueType = value.GetType();
            }
        }

        /// <summary>
        /// 是否是可序列化的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSerializableType(Type type)
        {
            bool isSerializable = HasSerializableAttribute(type);
            if (type.IsGenericType)
            {
                Type[] genericArgs= type.GetGenericArguments();
                foreach (Type genericArg in genericArgs)
                {
                    isSerializable = HasSerializableAttribute(genericArg);
                    if(!isSerializable)
                        break;
                }
            }

            return isSerializable;
        }

        /// <summary>
        /// 类型上是否有可序列化属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool HasSerializableAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof (SerializableAttribute), false).Count() > 0;
        }

        private static byte[] BinarySerialize(object value)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        public bool Set(string key, object value)
        {
            IRedisClient client = RedisFactory.CreateClient(key);
            if (client == null)
                return false;

            using (client)
            {
                if (IsSerializableType(value.GetType()))
                    return client.Set(key, BinarySerialize(value));

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
                if(IsSerializableType(value.GetType()))
                    return client.Set(key, BinarySerialize(value), expiresAt);

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
                if(IsSerializableType(value.GetType()))
                    return client.Set(key, BinarySerialize(value), expiresIn);
                
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
                byte[] buffer = client.Get<byte[]>(key);
                bool isDeserializable = false;
                try
                {
                    //先尝试进行二进制序列化
                    if (buffer != null)
                    {
                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            object obj = new BinaryFormatter().Deserialize(ms);
                            result = (T) obj;
                            isDeserializable = true;
                        }
                    }
                }
                catch
                {
                    isDeserializable = false;
                }

                if(!isDeserializable)
                {
                    ValueWrapper wrapper = client.Get<ValueWrapper>(key);
                    if (wrapper != null)
                    {
                        if (wrapper.Value.GetType() == wrapper.ValueType) //判断序列化的类型与实际类型是否一致
                            result = (T) wrapper.Value;
                        else
                        {
                            result = (T) JsonSerializer.DeserializeFromString(wrapper.Value.ToString(), wrapper.ValueType);// 不一致则按实际类型反序列化
                        }
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
