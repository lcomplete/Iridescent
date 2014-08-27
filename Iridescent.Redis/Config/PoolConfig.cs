using System.Configuration;

namespace Iridescent.Redis.Config
{
    /// <summary>
    /// 功能：Redis连接池配置结点
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public class PoolConfig:ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
        }

        [ConfigurationProperty("weight", IsRequired = true)]
        public int Weight
        {
            get { return int.Parse(this["weight"].ToString()); }
        }

        [ConfigurationProperty("hosts", IsRequired = true)]
        public string Hosts
        {
            get { return this["hosts"].ToString(); }
        }
    }
}