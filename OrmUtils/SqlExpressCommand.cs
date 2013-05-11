using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Iridescent.Data.QueryModel;
using System.Linq;

namespace Iridescent.OrmExpress
{
    /// <summary>
    /// 精简的Sql命令对象
    /// </summary>
    public class SqlExpressCommand
    {
        public CommandType CommandType { get; set; }
        public StringBuilder Statement { get; set; }
        public IList<DbParameter> DbParameters { get; set; }

        public string StatementString
        {
            get { return Statement != null ? Statement.ToString() : string.Empty; }
        }

        public void AppendStatement(string statement)
        {
            if (Statement == null)
            {
                Statement=new StringBuilder(20);
            }
            Statement.Append(statement);
        }

        public void AppendDbParameter(DbParameter dbParameter)
        {
            if (DbParameters == null)
            {
                DbParameters = new List<DbParameter>(20);
            }
            DbParameters.Add(dbParameter);
        }

        public void TranslateQuery(Query query)
        {
            if (CommandType!=CommandType.Text)
            {
                throw new InvalidOperationException("只能为普通sql语句翻译查询");
            }
            IQueryTranslator translator = QueryTranslatorFactory.Create(this, query);
            translator.Execute();
        }

        public DbParameter[] GetDbParameterArray()
        {
            if(DbParameters==null)
                return new DbParameter[0];
            return DbParameters.ToArray();
        }

        public SqlExpressCommand(StringBuilder statement, IList<DbParameter> sqlParameters):this(statement)
        {
            DbParameters = sqlParameters;
        }

        public SqlExpressCommand(StringBuilder statement):this()
        {
            Statement = statement;
        }

        public SqlExpressCommand()
        {
            CommandType = CommandType.Text;
        }

        public static SqlExpressCommand Create(string sqlStatement,Query query)
        {
            SqlExpressCommand command = new SqlExpressCommand();
            command.AppendStatement(sqlStatement);
            command.TranslateQuery(query);
            return command;
        }
    }
}