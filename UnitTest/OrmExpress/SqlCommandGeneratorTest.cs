using System;
using Iridescent.Data.QueryModel;
using Iridescent.Entities;
using Iridescent.OrmExpress;
using NUnit.Framework;

namespace UnitTest.OrmExpress
{

    [TestFixture]
    public class SqlCommandGeneratorTest
    {
        public SqlCommandGeneratorTest()
        {
        }

        [Test]
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
                command = commandGenerator.GenerateSelectCommand<Goods>(query);
            else if (type == "D")
                command = commandGenerator.GenerateDeleteCommand<Goods>(query);
            return command.Statement.ToString();
        }

        [Test]
        public void GenerateDeleteCommand()
        {
            GenerateCommand("D");
        }

        [Test]
        public void GenerateUpdateCommand()
        {
            Goods goods=new Goods();
            SqlExpressCommand command = SqlExpressCommandGeneratorFactory.Create().GenerateUpdateCommand(goods);
            Console.WriteLine(command.Statement.ToString());
        }

        [Test]
        public void GenerateInsertCommand()
        {
            Goods goods = new Goods();
            SqlExpressCommand command = SqlExpressCommandGeneratorFactory.Create().GenerateInsertCommand(goods);
            Console.WriteLine(command.Statement.ToString());
        }
    }
}
