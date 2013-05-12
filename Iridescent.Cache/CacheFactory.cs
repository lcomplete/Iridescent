using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Iridescent.Cache
{
    public static class CacheFactory
    {
        public static ICache Create()
        {
            string cacheProvider = ConfigurationManager.AppSettings["CacheProvider"];
            ICache cache = null;
            if(!string.IsNullOrWhiteSpace(cacheProvider))
            {
                cache = (ICache) Activator.CreateInstance(Type.GetType(cacheProvider));
            }
            else
                cache=new WebCache();

            return cache;
        }
    }
}
