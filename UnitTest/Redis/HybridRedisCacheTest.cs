using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Iridescent.Redis;
using NUnit.Framework;

namespace UnitTest.Redis
{
    [TestFixture]
    class HybridRedisCacheTest
    {
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

            DataTable dt=new DataTable();
            dt.Columns.Add("a");
            for (int i = 0; i < 10000; i++)
            {
                DataRow row = dt.NewRow();
                row["a"] = new string('中', 1000);
                dt.Rows.Add(row);
            }
            CacheIt(dt);

            CacheIt(new DataSet());
            CacheIt(new List<string>() { "abc" });

            List<TestClass> tc=new List<TestClass>();
            for (int i = 0; i < 10000; i++)
            {
                tc.Add(new TestClass());
            }
            CacheIt(tc);

            CacheIt(new TestClass());
        }

        private void CacheIt<T>(T value = default(T))
        {
            Console.WriteLine("------------------");
            Stopwatch sw=new Stopwatch();
            
            var redis = new HybridRedisCache();
            string key = typeof(T).Name;

            sw.Start();
            redis.Set(key, value, TimeSpan.FromSeconds(60));
            sw.Stop();
            Console.WriteLine("set:"+sw.Elapsed);

            sw.Reset();
            sw.Start();
            object cacheResult = redis.Get(key);
            sw.Stop();
            Console.WriteLine("get:"+ sw.Elapsed);


            Console.WriteLine(cacheResult.GetType() + ", " + cacheResult);
            Assert.AreEqual(typeof(T), cacheResult.GetType());
        }
    }
}
