using System;
using System.Collections.Generic;
using System.Threading;
using ServiceStack.Redis;

namespace Iridescent.Redis
{
    /// <summary>
    /// 功能：Redis客户端连接分区管理对象 （包含自动检测并管理Redis连接池）
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public class AutoDetectShardedRedisClientManager
    {
        /// <summary>
        /// 一致性Hash对象（提供Hash映射算法）
        /// </summary>
        private readonly RemovableConsistentHash<ShardedConnectionPool> consistentHash;

        /// <summary>
        /// 已移除的redis结点
        /// </summary>
        private IList<KeyValuePair<ShardedConnectionPool, int>> removedNodes = new List<KeyValuePair<ShardedConnectionPool, int>>();

        public AutoDetectShardedRedisClientManager(params ShardedConnectionPool[] connectionPools)
        {
            if (connectionPools == null)
                throw new ArgumentNullException("connectionPools", "连接池不能为空。");
            var pools = new List<KeyValuePair<ShardedConnectionPool, int>>(); // redis 连接池
            foreach (ShardedConnectionPool key in connectionPools)
                pools.Add(new KeyValuePair<ShardedConnectionPool, int>(key, key.weight));
            consistentHash = new RemovableConsistentHash<ShardedConnectionPool>(pools);

            AutoDetectShardedPool(pools);
        }

        /// <summary>
        /// 通过key映射得到redis连接池对象
        /// </summary>
        /// <param name="key">用于一致性hash映射的key</param>
        /// <returns>
        /// redis连接池对象
        /// </returns>
        public ShardedConnectionPool GetConnectionPool(string key)
        {
            return consistentHash.GetTarget(key);
        }

        /// <summary>
        /// 启动自动检测进程
        /// </summary>
        /// <param name="pools"></param>
        public void AutoDetectShardedPool(IList<KeyValuePair<ShardedConnectionPool, int>> pools)
        {
            Thread detectThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        checkValidPools(pools);
                    }
                    catch (Exception)
                    {
                        //TODO 记录日志
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(60));
                    try
                    {
                        checkInvalidPools();
                    }
                    catch (Exception)
                    {
                        //TODO 记录日志
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(60));
                }
            });
            detectThread.Start();
        }

        /// <summary>
        /// 检查可用连接池 （不可用则移除）
        /// </summary>
        /// <param name="pools"></param>
        private void checkValidPools(IEnumerable<KeyValuePair<ShardedConnectionPool, int>> pools)
        {
            foreach (var pool in pools)
            {
                using (var writeclient = pool.Key.GetClient())
                using (var readclient = pool.Key.GetReadOnlyClient())
                {
                    var nwclient = writeclient as RedisNativeClient;
                    var nrclient = readclient as RedisNativeClient;
                    bool needRemove = false;
                    try
                    {
                        if (!nwclient.Ping())
                        {
                            if (!nrclient.Ping())
                            {
                                needRemove = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        needRemove = true; //访问出错则移除
                    }

                    if (needRemove)
                    {
                        removedNodes.Add(pool);
                        lock (consistentHash)
                        {
                            consistentHash.RemoveTarget(pool.Key, pool.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检查不可用连接池 (发现变的可用，则恢复)
        /// </summary>
        private void checkInvalidPools()
        {
            var addBack = new List<KeyValuePair<ShardedConnectionPool, int>>();
            foreach (var pool in removedNodes)
            {
                using (var writeclient = pool.Key.GetClient())
                using (var readclient = pool.Key.GetReadOnlyClient())
                {
                    var nwclient = writeclient as RedisNativeClient;
                    var nrclient = readclient as RedisNativeClient;
                    if (nwclient.Ping())
                    {
                        if (nrclient.Ping())
                        {
                            lock (consistentHash)
                            {
                                addBack.Add(pool);
                                consistentHash.AddTarget(pool.Key, pool.Value);
                            }
                        }
                    }
                }
            }

            foreach (var pool in addBack)
            {
                removedNodes.Remove(pool);
            }
        }
    }
}