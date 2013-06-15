using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Examples.IService;
using Examples.Service;
using Iridescent.CacheHandler;
using Iridescent.DependencyResolution;
using NUnit.Framework;

namespace UnitTest.CacheHandler
{
    [TestFixture]
    public class CachingHandlerTest
    {
        [SetUp]
        public void Setup()
        {
            ContainerFactory.Singleton.Init(new[] { "Examples.IService", "Examples.Service" });
            CacchingAopConfig.Config("Examples.IService");
        }

        [Test]
        public void ShouldInjection()
        {
            ITestService service = ContainerFactory.Singleton.Resolve<ITestService>();
            DateTime currentDate = service.GetCurrentDate(0);
            Thread.Sleep(100);
            DateTime cacheDate = service.GetCurrentDate(0);
            DateTime anotherCacheDate = service.GetCurrentDate(1);

            Console.WriteLine(currentDate);
            Console.WriteLine(cacheDate);
            Console.WriteLine(anotherCacheDate);

            Assert.AreEqual(currentDate, cacheDate);
            Assert.AreNotEqual(cacheDate, anotherCacheDate);
        }

    }
}
