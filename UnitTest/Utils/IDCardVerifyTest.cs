using Iridescent.Utils.Verify;
using NUnit.Framework;

namespace UnitTest.Utils
{
    
    
    /// <summary>
    ///这是 IDCardVerifyTest 的测试类，旨在
    ///包含所有 IDCardVerifyTest 单元测试
    ///</summary>
    [TestFixture]
    public class IDCardVerifyTest
    {

        /// <summary>
        ///CheckIDCard 的测试
        ///</summary>
        [Test]
        public void CheckIDCardTest()
        {
            string Id = "112502190000201234"; 
            bool expected = false; 
            bool actual;
            actual = IDCardVerify.CheckIDCard(Id);
            Assert.AreEqual(expected, actual);
        }

    }
}
