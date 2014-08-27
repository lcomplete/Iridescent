using System.Configuration;

namespace Iridescent.Redis.Config
{
    /// <summary>
    /// 功能：Redis配置根节点
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public class RedisConfigurationSection:ConfigurationSection
    {
        [ConfigurationProperty("pools")]
        public Pools Pools
        {
            get { return base["pools"] as Pools; }
        }
    }
}