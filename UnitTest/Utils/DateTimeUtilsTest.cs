using Iridescent.Utils;
using System;
using Iridescent.Utils.Common;
using NUnit.Framework;

namespace UnitTest.Utils
{


    /// <summary>
    ///这是 DateTimeUtilsTest 的测试类，旨在
    ///包含所有 DateTimeUtilsTest 单元测试
    ///</summary>
    [TestFixture]
    public class DateTimeUtilsTest
    {

        /// <summary>
        ///DiffYearWithFloor 的测试
        ///</summary>
        [Test]
        public void DiffYearWithFloorTest()
        {
            DateTime dateTime1 = new DateTime(1977, 4, 26);
            DateTime dateTime2 = new DateTime(2012, 4, 25);
            int expected = 34;
            int actual;
            actual = DateTimeUtils.DiffYearWithFloor(dateTime1, dateTime2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///DiffYearWithFloor 的测试
        ///</summary>
        [Test]
        public void DiffYearWithFloor2Test()
        {
            DateTime dateTime1 = new DateTime(1995, 4, 4);
            DateTime dateTime2 = new DateTime(1993, 4, 2);
            int expected = 2;
            int actual;
            actual = DateTimeUtils.DiffYearWithFloor(dateTime1, dateTime2);
            Assert.AreEqual(expected, actual);
        }
    }
}
