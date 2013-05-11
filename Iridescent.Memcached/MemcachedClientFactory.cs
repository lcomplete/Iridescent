using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;

namespace Iridescent.Memcached
{
    internal static class MemcachedClientFactory
    {
        //MemcachedClient 是一个重量级的对象 每次调用都创建和销毁会降低性能 
        //https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Usage
        
        private static IMemcachedClient _client;

        static MemcachedClientFactory()
        {
            _client = Create();
        }

        private static readonly object _syncRoot=new object();

        /// <summary>
        /// 使用单例模式 创建MemcachedClient客户端实例
        /// </summary>
        /// <returns></returns>
        public static IMemcachedClient Create()
        {
            if(_client==null)
            {
                lock (_syncRoot)
                {
                    if(_client==null)
                    {
                        _client=new MemcachedClient();
                    }
                }
            }

            return _client;
        }
    }
}
