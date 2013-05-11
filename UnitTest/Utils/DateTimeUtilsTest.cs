using Iridescent.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest.Utils
{


    /// <summary>
    ///这是 DateTimeUtilsTest 的测试类，旨在
    ///包含所有 DateTimeUtilsTest 单元测试
    ///</summary>
    [TestClass()]
    public class DateTimeUtilsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///DiffYearWithFloor 的测试
        ///</summary>
        [TestMethod()]
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
        [TestMethod()]
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
