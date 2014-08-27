using System.Configuration;
using Iridescent.Redis.Config;
using ServiceStack.Redis;

namespace Iridescent.Redis
{
    /// <summary>
    /// 功能：Redis客户端工厂
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public static class RedisFactory
    {
        /// <summary>
        /// Redis客户端连接分区管理对象 （利用一致性哈希算法提供客户端连接分区）
        /// </summary>
        private static readonly AutoDetectShardedRedisClientManager RedisClientManager;

        static RedisFactory()
        {
            var redisConfig = ConfigurationManager.GetSection("redisSection") as RedisConfigurationSection;
            if (redisConfig != null)
            {
                var connectionPools = new ShardedConnectionPool[redisConfig.Pools.Count];
                int index = 0;
                foreach (PoolConfig poolConfig in redisConfig.Pools)
                {
                    connectionPools[index] = new ShardedConnectionPool(poolConfig.Name, poolConfig.Weight,
                        poolConfig.Hosts);
                    index++;
                }
                RedisClientManager = new AutoDetectShardedRedisClientManager(connectionPools);
            }
        }

        /// <summary>
        /// 通过Key映射得到RedisClient对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IRedisClient CreateClient(string key)
        {
            if (RedisClientManager == null)
                return null;

            ShardedConnectionPool pool= RedisClientManager.GetConnectionPool(key); //通过key映射到指定的连接池
            return pool != null ? pool.GetClient() : null;
        }
    }
}