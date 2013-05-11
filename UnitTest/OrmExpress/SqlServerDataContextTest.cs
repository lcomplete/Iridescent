using Iridescent.Data;
using Iridescent.Entities;
using Iridescent.OrmExpress;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Iridescent.Data.QueryModel;

namespace UnitTest.OrmExpress
{
    
    
    /// <summary>
    ///这是 SqlServerDataContextTest 的测试类，旨在
    ///包含所有 SqlServerDataContextTest 单元测试
    ///</summary>
    [TestClass()]
    public class SqlServerDataContextTest
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
        ///GetAll 的测试
        ///</summary>
        public void GetAllTestHelper<T>()
            where T : class , new()
        {
            SqlServerDataContext target = GetDataContext(); 
            IList<T> actual;
            
            actual = target.GetAll<T>();
            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            GetAllTestHelper<Temp>();
        }

        [TestMethod()]
        public void GetPagingListTest()
        {
            IDataContext dataContext = GetDataContext();
            IList<Temp> temps =
                dataContext.GetByCriteria<Temp>(new Query(new Criterion("Id", CriteriaOperator.GreaterThan, 2)), 2, 1);
            Assert.IsTrue(temps.Count>0);
        }

        [TestMethod]
        public void GetCountTest()
        {
            IDataContext dataContext = GetDataContext();
            int count =
                dataContext.GetCount<Temp>(new Query(new Criterion("Id", CriteriaOperator.GreaterThanOrEqual, 3)));
            Assert.IsTrue(count>0);
        }

        /// <summary>
        ///GetByCriteria 的测试
        ///</summary>
        public void GetByCriteriaTestHelper<T>()
            where T : class , new()
        {
            SqlServerDataContext target = GetDataContext(); 
            Query query = new Query(new Criterion("What",CriteriaOperator.Like,"what"),  new OrderClause("What", OrderClause.OrderClauseCriteria.Ascending));
            IList<T> actual;
            actual = target.GetByCriteria<T>(query);
            Assert.IsNotNull(actual);
        }

        [TestMethod()]
        public void GetByCriteriaTest()
        {
            GetByCriteriaTestHelper<Temp>();
        }

        /// <summary>
        ///GetById 的测试
        ///</summary>
        public void GetByIdTestHelper<T>()
            where T : class , new()
        {
            SqlServerDataContext target = GetDataContext(); 
            object key = 2; 
            T actual;
            actual = target.GetById<T>(key);
            Assert.IsNotNull(actual);
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            GetByIdTestHelper<Temp>();
        }

        /// <summary>
        ///Delete 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            SqlServerDataContext target = GetDataContext(); 
            Temp item = target.GetById<Temp>(1) ; 
            target.Delete(item);
        }

        /// <summary>
        ///Add 的测试
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            SqlServerDataContext target = GetDataContext(); 
            Temp item = new Temp() { What = "what2" }; ;
            target.Add(item);
        }

        /// <summary>
        ///Save 的测试
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            SqlServerDataContext target = GetDataContext(); 
            Temp item = new Temp(){Id=1,What="what1"}; 
            target.Save(item);
        }

        public static SqlServerDataContext GetDataContext()
        {
            return (SqlServerDataContext)(new DataAccessProviderFactory().GetDataContext());
        }

        [TestMethod]
        public void TranscationTest()
        {
            IDataContext dataContext = GetDataContext();
            try
            {
                dataContext.BeginTransaction();
                dataContext.Add(new Temp() { What = "abbbbbb11" });
                dataContext.Delete(new Temp() {Id = 8});
                dataContext.Add(new Temp()
                                    {
                                        What =
                                            "abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11abbbbbb11"
                                    });
                dataContext.Commit();
            }
            catch
            {
                dataContext.Rollback();
            }
        }
    }
}
