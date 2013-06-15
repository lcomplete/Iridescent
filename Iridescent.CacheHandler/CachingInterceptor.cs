using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Castle.DynamicProxy;
using Iridescent.Cache;
using Snap;

namespace Iridescent.CacheHandler
{
    public class CachingInterceptor : MethodInterceptor
    {

        private ICacheKeyGenerator _keyGenerator;

        public CachingInterceptor()
        {
            _keyGenerator = new DefaultCacheKeyGenerator();
        }

        public override void InterceptMethod(IInvocation invocation, MethodBase method, Attribute attribute)
        {
            CachingAttribute cachingAttribute = attribute as CachingAttribute;
            if (cachingAttribute == null)
            {
                invocation.Proceed();
            }
            else
            {
                string cacheKey = _keyGenerator.CreateCacheKey(method, invocation.Arguments);
                CacheManager cacheManager = new CacheManager(cachingAttribute.Group);
                object cachedResult = cacheManager.Get(cacheKey);
                if(cachedResult==null)
                {
                    lock (method)
                    {
                        cachedResult = cacheManager.Get(cacheKey);
                        if (cachedResult == null)
                        {
                            invocation.Proceed();
                            cachedResult = invocation.ReturnValue;
                            cacheManager.Set(cacheKey, cachedResult, cachingAttribute.CacheTimeSpan);
                        }
                    }
                }

                invocation.ReturnValue = cachedResult;
            }

        }

    }
}
