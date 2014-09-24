using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Iridescent.Cache
{
    public static class CacheFactory
    {
        private static readonly Dictionary<string, ICache> DictCacheProviders = new Dictionary<string, ICache>();

        private static readonly object SyncRoot = new object();

        public static ICache CreateDefault()
        {
            string cacheProvider = ConfigurationManager.AppSettings["CacheProvider"] ?? string.Empty;
            ICache cache = null;

            if (!DictCacheProviders.TryGetValue(cacheProvider, out cache))
            {
                lock (SyncRoot)
                {
                    if (!DictCacheProviders.TryGetValue(cacheProvider, out cache))
                    {
                        if (!string.IsNullOrWhiteSpace(cacheProvider))
                        {
                            cache = (ICache)Activator.CreateInstance(Type.GetType(cacheProvider));
                        }
                        else
                            cache = new WebCache();
                        DictCacheProviders.Add(cacheProvider, cache);
                    }
                }
            }

            return cache;
        }
    }
}
