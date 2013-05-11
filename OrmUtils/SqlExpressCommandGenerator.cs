using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Iridescent.Data.QueryModel;

namespace Iridescent.OrmExpress
{
    internal class SqlExpressCommandGenerator:ISqlExpressCommandGenerator
    {

        public SqlExpressCommand GenerateSelectCommand<TEntity>(Query query = null,bool getCount=false)
        {
            Type type = typeof (TEntity);
            StringBuilder stringBuilder=new StringBuilder(20);
            stringBuilder.Append("SELECT ");
            stringBuilder.Append(getCount ? "COUNT(1)" : string.Join(",", ReflectHelper.GetPropertyNames(type)));
            stringBuilder.Append(" FROM ");
            stringBuilder.Append(GetTableName(type));
            SqlExpressCommand sqlCommand = new SqlExpressCommand(stringBuilder);
            sqlCommand.TranslateQuery(query);
            return sqlCommand;
        }

        public SqlExpressCommand GenerateGetPagingListCommand<TEntity>(int pageIndex, int pageSize,Query query=null)
        {
            if (query == null)
            {
                query=new Query();
            }
            if (query.OrderClauses.Count == 0)
            {
                query.OrderClauses.Add(new OrderClause(PrimaryKeyFinder.GetPrimaryKey<TEntity>(),OrderClause.OrderClauseCriteria.Ascending));
            }
            Type type = typeof (TEntity);
            SqlExpressCommand sqlCommand=new SqlExpressCommand();
            IQueryTranslator queryTranslator = QueryTranslatorFactory.Create(sqlCommand, query);
            string fields = string.Join(",", ReflectHelper.GetPropertyNames(type));
            sqlCommand.AppendStatement("SELECT ");
            sqlCommand.AppendStatement(fields);
            sqlCommand.AppendStatement(" FROM (SELECT ROW_NUMBER() OVER(");
            queryTranslator.TranslateOrderClauses();
            sqlCommand.AppendStatement(") AS ROW_NUMBER,");
            sqlCommand.AppendStatement(fields);
            sqlCommand.AppendStatement(" FROM ");
            sqlCommand.AppendStatement(GetTableName(type));
            queryTranslator.TranslateCriterias();
            sqlCommand.AppendStatement(") AS T0 WHERE ROW_NUMBER BETWEEN ");
            sqlCommand.AppendStatement((((pageIndex - 1)*pageSize) + 1).ToString());
            sqlCommand.AppendStatement(" AND ");
            sqlCommand.AppendStatement((pageIndex*pageSize).ToString());
            return sqlCommand;
        }

        private string GetTableName(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof (TableAttribute), true);
            if (attributes.Length == 1)
            {
                TableAttribute tableAttribute = attributes[0] as TableAttribute;
                if (tableAttribute != null && !string.IsNullOrEmpty(tableAttribute.Name))
                    return tableAttribute.Name;
            }

            return type.Name;
        }

        public SqlExpressCommand GenerateDeleteCommand<TEntity>(Query query = null)
        {
            Type type = typeof (TEntity);
            return GenerateDeleteCommand(type, query);
        }

        private SqlExpressCommand GenerateDeleteCommand(Type type, Query query)
        {
            StringBuilder stringBuilder = new StringBuilder(20);
            stringBuilder.Append("DELETE FROM ");
            stringBuilder.Append(GetTableName(type));
            SqlExpressCommand sqlCommand = new SqlExpressCommand(stringBuilder);
            sqlCommand.TranslateQuery(query);
            return sqlCommand;
        }

        public SqlExpressCommand GenerateDeleteCommand<TEntity>(TEntity entity)
        {
            string primaryKey = PrimaryKeyFinder.GetPrimaryKey(entity.GetType());
            Query query = new Query();
            query.Criteria.Add(new Criterion(primaryKey,CriteriaOperator.Equal,ReflectHelper.GetPropertyValue(entity,primaryKey)));
            return GenerateDeleteCommand(entity.GetType(),query);
        }

        public SqlExpressCommand GenerateUpdateCommand<TEntity>(TEntity entity)
        {
            SqlExpressCommand command=new SqlExpressCommand();
            command.Statement=new StringBuilder(40);

            Type type = entity.GetType();
            command.AppendStatement("UPDATE ");
            command.AppendStatement(GetTableName(type));
            command.AppendStatement(" SET ");

            string[] propertyNames = ReflectHelper.GetPropertyNames(type);
            string primaryKey = PrimaryKeyFinder.GetPrimaryKey(entity.GetType());

            bool isFirstField = true;
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (string.Equals(propertyNames[i], primaryKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                
                if (isFirstField)
                {
                    isFirstField = false;
                }
                else
                {
                    command.AppendStatement(",");
                }

                AppendUpdateSqlAndParameter(i, propertyNames[i], entity,command);
            }

            command.AppendStatement(" WHERE ");
            AppendUpdateSqlAndParameter(propertyNames.Length,primaryKey,entity,command);

            return command;
        }

        private void AppendUpdateSqlAndParameter(int index, string propertyName, object entity, SqlExpressCommand command)
        {
            string parameterName = "@p" + index.ToString();
            command.AppendStatement(propertyName);
            command.AppendStatement("=");
            command.AppendStatement(parameterName);
            command.AppendDbParameter(new SqlParameter(parameterName, ReflectHelper.GetPropertyValue(entity, propertyName)));
        }

        public SqlExpressCommand GenerateInsertCommand<TEntity>(TEntity entity)
        {
            SqlExpressCommand command=new SqlExpressCommand();
            command.Statement=new StringBuilder(40);

            Type type = entity.GetType();
            command.AppendStatement("INSERT INTO ");
            command.AppendStatement(GetTableName(type));

            string primaryKey =PrimaryKeyFinder.GetPrimaryKey(entity.GetType());
            string[] propertyNames = ReflectHelper.GetPropertyNames(type);

            AppendInsertFileds(propertyNames, primaryKey, command);
            command.AppendStatement(" VALUES ");
            AppendInsertSqlAndParameters(propertyNames,primaryKey,command,entity);

            command.AppendStatement(";SELECT @@IDENTITY;");

            return command;
        }

        private void AppendInsertFileds(string[] propertyNames, string primaryKey, SqlExpressCommand command)
        {
            AppendInsertColumns(propertyNames,primaryKey,command,null);
        }

        private void AppendInsertSqlAndParameters(string[] propertyNames, string primaryKey, SqlExpressCommand command, object entity)
        {
            AppendInsertColumns(propertyNames,primaryKey,command,entity);
        }

        private void AppendInsertColumns(string[] propertyNames, string primaryKey, SqlExpressCommand command, object entity)
        {
            command.AppendStatement("(");
            bool isFirstField = true;
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (string.Equals(propertyNames[i], primaryKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (isFirstField)
                {
                    isFirstField = false;
                }
                else
                {
                    command.AppendStatement(",");
                }

                if (entity == null)
                {
                    command.AppendStatement(propertyNames[i]);
                }
                else
                {
                    string parameterName = "@p" + i.ToString();
                    command.AppendStatement(parameterName);
                    command.AppendDbParameter(new SqlParameter(parameterName,ReflectHelper.GetPropertyValue(entity,propertyNames[i])));
                }
            }
            command.AppendStatement(")");
        }
    }
}