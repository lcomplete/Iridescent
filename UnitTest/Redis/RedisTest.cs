using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridescent.Redis;
using NUnit.Framework;

namespace UnitTest.Redis
{
    [TestFixture]
    class RedisTest
    {
        [Test]
        public void EnsureRedisWork()
        {
            RedisCache cache = new RedisCache();
            string key = "now";
            DateTime dt = cache.Get<DateTime>(key);
            if (dt == DateTime.MinValue)
            {
                dt = DateTime.Now;
                cache.Set(key, dt, TimeSpan.FromMinutes(10));
            }
            DateTime dt1 = cache.Get<DateTime>(key);
            Console.WriteLine(dt);
            Console.WriteLine(dt1);
        }
    }
}
