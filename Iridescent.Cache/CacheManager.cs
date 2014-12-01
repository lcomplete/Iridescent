using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Iridescent.Cache
{
    public static class CacheManager
    {
        private static string CachePrefix
        {
            get { return ConfigurationManager.AppSettings["CachePrefix"] ?? string.Empty; }
        }

        private static string WrapKey(string key)
        {
            return CachePrefix + key;
        }

        /// <summary>
        /// 获取缓存对象，若不存在则执行委托方法并缓存结果
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="ifnotfound">数据获取委托</param>
        /// <param name="cacheMinutes">缓存分钟</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> ifnotfound, int cacheMinutes)
            where T : class
        {
            return Get(key, ifnotfound, TimeSpan.FromMinutes(cacheMinutes), null);
        }

        /// <summary>
        /// 获取缓存对象，若不存在则执行委托方法，通过条件则进行缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="ifnotfound">数据获取委托</param>
        /// <param name="cacheMinutes">缓存分钟</param>
        /// <param name="conditionFunc">缓存条件委托</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> ifnotfound, int cacheMinutes, Func<T, bool> conditionFunc)
            where T : class
        {
            return Get(key, ifnotfound, TimeSpan.FromMinutes(cacheMinutes), conditionFunc);
        }

        /// <summary>
        /// 获取缓存对象，若不存在则执行委托方法并缓存结果
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="ifnotfound">数据获取委托</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> ifnotfound, TimeSpan cacheTime) where T : class
        {
            return Get(key, ifnotfound, cacheTime, null);
        }

        /// <summary>
        /// 获取缓存对象，若不存在则执行委托方法，通过条件则进行缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="ifnotfound">数据获取委托</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <param name="conditionFunc">缓存条件委托</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> ifnotfound, TimeSpan cacheTime, Func<T, bool> conditionFunc)
            where T : class
        {
            ICache cache = CacheFactory.CreateDefault();
            string wrapKey = WrapKey(key);
            T obj = cache.Get<T>(wrapKey);
            if (obj == null)
            {
                obj = ifnotfound.Invoke();
                if (obj != null)
                {
                    if (conditionFunc == null || conditionFunc(obj)) //不存在条件或者条件通过 进行缓存
                        cache.Set(wrapKey, obj, cacheTime);
                }
            }

            return obj;
        }

        #region CacheProvider 委托

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存对象</param>
        /// <returns></returns>
        public static bool Set(string key, object value)
        {
            ICache cache = CacheFactory.CreateDefault();
            return cache.Set(WrapKey(key), value);
        }

        /// <summary>
        /// 设置缓存 指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存对象</param>
        /// <param name="expiresAt">过期时间</param>
        /// <returns></returns>
        public static bool Set(string key, object value, DateTime expiresAt)
        {
            ICache cache = CacheFactory.CreateDefault();
            return cache.Set(WrapKey(key), value, expiresAt);
        }

        /// <summary>
        /// 设置缓存 指定缓存时间段
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存对象</param>
        /// <param name="expiresIn">缓存时间</param>
        /// <returns></returns>
        public static bool Set(string key, object value, TimeSpan expiresIn)
        {
            ICache cache = CacheFactory.CreateDefault();
            return cache.Set(WrapKey(key), value, expiresIn);
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            ICache cache = CacheFactory.CreateDefault();
            return cache.Get<T>(WrapKey(key));
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static object Get(string key) 
        {
            ICache cache = CacheFactory.CreateDefault();
            return cache.Get(WrapKey(key));
        }

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key">缓存对象</param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            ICache cache = CacheFactory.CreateDefault();
            return cache.Remove(WrapKey(key));
        }

        #endregion
    }
}
