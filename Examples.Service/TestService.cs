using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Examples.IService;
using Iridescent.CacheHandler;

namespace Examples.Service
{
    public class TestService: ITestService
    {
        [Caching]
        public DateTime GetCurrentDate(int i)
        {
            return DateTime.Now;
        }
    }
}
