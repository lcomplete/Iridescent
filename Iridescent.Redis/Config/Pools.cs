using System.Configuration;

namespace Iridescent.Redis.Config
{
    /// <summary>
    /// 功能：Redis配置结点组
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public class Pools : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PoolConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as PoolConfig).Name;
        }
    }
}