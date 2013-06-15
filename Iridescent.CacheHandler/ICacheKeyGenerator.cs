using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Iridescent.CacheHandler
{
    interface ICacheKeyGenerator
    {
        string CreateCacheKey(MethodBase method, object[] inputs);
    }
}
