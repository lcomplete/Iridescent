using System;
using System.Reflection;
using Castle.DynamicProxy;
using Snap.StructureMap;
using StructureMap;
using Snap;

namespace Iridescent.CacheHandler
{

    public static class CacchingAopConfig
    {
        public static void Config(string includeNamespace)
        {
            SnapConfiguration.For<StructureMapAspectContainer>(c =>
            {
                c.IncludeNamespace(includeNamespace);
                c.Bind<CachingInterceptor>().To<CachingAttribute>();
            });
        }

    }

}
