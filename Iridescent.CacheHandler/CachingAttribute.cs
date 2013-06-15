using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snap;

namespace Iridescent.CacheHandler
{
    public class CachingAttribute : MethodInterceptAttribute
    {
        public TimeSpan CacheTimeSpan { get; private set; }

        public string Group { get; private set; }

        public CachingAttribute(int minutes = 30, string group = "")
            : this(TimeSpan.FromMinutes(minutes), group)
        {
        }

        public CachingAttribute(TimeSpan timeSpan, string group = "")
        {
            CacheTimeSpan = timeSpan;
            Group = group;
        }
    }
}
