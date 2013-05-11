using System;
using Iridescent.Data.QueryModel;
using Iridescent.Entities;
using Iridescent.OrmExpress;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.OrmExpress
{

    [TestClass]
    public class SqlCommandGeneratorTest
    {
        public SqlCommandGeneratorTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
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
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GenerateSelectCommand()
        {
            GenerateCommand("R");
        }

        private string GenerateCommand(string type)
        {
            ISqlExpressCommandGenerator commandGenerator = SqlExpressCommandGeneratorFactory.Create();
            Query query = new Query();
            query.Criteria.Add(new Criterion("GoodsName", CriteriaOperator.Like, "marcus"));
            Query subQuery = new Query();
            query.SubQueries.Add(subQuery);
            query.Operator = QueryOperator.Or;
            subQuery.Criteria.Add(new Criterion("GoodsId", CriteriaOperator.GreaterThan, 90000));
            SqlExpressCommand command=null;
            if (type == "R")
                command = commandGenerator.GenerateSelectCommand<Bhg_Goods>(query);
            else if (type == "D")
                command = commandGenerator.GenerateDeleteCommand<Bhg_Goods>(query);
            return command.Statement.ToString();
        }

        [TestMethod]
        public void GenerateDeleteCommand()
        {
            GenerateCommand("D");
        }

        [TestMethod]
        public void GenerateUpdateCommand()
        {
            Bhg_Goods goods=new Bhg_Goods();
            SqlExpressCommand command = SqlExpressCommandGeneratorFactory.Create().GenerateUpdateCommand(goods);
            Console.WriteLine(command.Statement.ToString());
        }

        [TestMethod]
        public void GenerateInsertCommand()
        {
            Bhg_Goods goods = new Bhg_Goods();
            SqlExpressCommand command = SqlExpressCommandGeneratorFactory.Create().GenerateInsertCommand(goods);
            Console.WriteLine(command.Statement.ToString());
        }
    }
}
