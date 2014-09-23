using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Iridescent.Redis;
using NUnit.Framework;

namespace UnitTest.Redis
{
    [TestFixture]
    class BinaryRedisCacheTest
    {
        [Serializable]
        class TestClass
        {
            public DateTime DateTime { get; set; } 
        }

        [Test]
        public void ShouldCacheUsualType()
        {
            CacheIt<int>();
            CacheIt<string>("abc");
            CacheIt<long>();
            CacheIt<DateTime>();
            CacheIt<double>();
            CacheIt<float>();
            CacheIt(new StringBuilder("builder"));
            CacheIt(new DataTable());
            CacheIt(new DataSet());
            CacheIt(new List<string>(){"abc"});
            CacheIt(new List<TestClass>(){new TestClass()});
            CacheIt(new TestClass());
        }

        private void CacheIt<T>(T value=default(T))
        {
            BinaryRedisCache redis=new BinaryRedisCache();
            string key = typeof (T).Name;
            redis.Set(key, value, TimeSpan.FromSeconds(10));
            object cacheResult= redis.Get(key);
            Console.WriteLine(cacheResult.GetType()+", "+cacheResult);
            Assert.AreEqual(typeof(T),cacheResult.GetType());
        }
    }
}
