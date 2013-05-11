using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Iridescent.Data.QueryModel;

namespace Iridescent.OrmExpress
{
    internal class SqlServerQueryTranslator:IQueryTranslator
    {
        private SqlExpressCommand _sqlCommand;
        private Query _query;

        public SqlServerQueryTranslator(SqlExpressCommand sqlCommand, Query query)
        {
            _sqlCommand = sqlCommand;
            _query = query;
        }

        public void Execute()
        {
            if (_query != null)
            {
                TranslateCriterias();
                TranslateOrderClauses();
            }
        }

        public void TranslateCriterias()
        {
            if (_query!=null && _query.Criteria.Count > 0)
            {
                _sqlCommand.AppendStatement(" WHERE ");
                int parameterIndex = 0;
                RecursiveAppendCondition(ref parameterIndex);
            }
        }

        public void TranslateOrderClauses()
        {
            if (_query != null && _query.OrderClauses.Count > 0)
            {
                _sqlCommand.AppendStatement(" ORDER BY ");
                for (int i = 0; i < _query.OrderClauses.Count; i++)
                {
                    if(i>0)
                        _sqlCommand.AppendStatement(",");
                    OrderClause clause = _query.OrderClauses[i];
                    _sqlCommand.AppendStatement("[");
                    _sqlCommand.AppendStatement(clause.PropertyName);
                    _sqlCommand.AppendStatement("] ");
                    _sqlCommand.AppendStatement(clause.Criterion == OrderClause.OrderClauseCriteria.Ascending ? "ASC" : "DESC");
                }
            }
        }

        private void RecursiveAppendCondition(ref int parameterIndex)
        {
            Query myQuery = this._query;

            _sqlCommand.AppendStatement("(");
            for (int i = 0; i < myQuery.Criteria.Count; i++)
            {
                Criterion myCriterion = myQuery.Criteria[i];
                string parameterName = "@p" + parameterIndex.ToString();
                CriteriaOperator criteriaOperator = myCriterion.Operator;

                if (i > 0)
                {
                    _sqlCommand.AppendStatement(" " + myQuery.Operator.ToString() + " ");// 添加and或or
                }
                _sqlCommand.AppendStatement(myCriterion.PropertyName);//添加条件字段
                _sqlCommand.AppendStatement(TranslateCriteriaOperator(criteriaOperator));//添加操作符
                if (criteriaOperator == CriteriaOperator.IsNull || criteriaOperator == CriteriaOperator.IsNotNull)//判断NULL时 不需要继续添加参数
                {
                    continue;
                }
                AppendPercentSymbolForLikeOperator(criteriaOperator,true);//为like条件添加'%'
                _sqlCommand.AppendStatement(parameterName);//添加参数
                AppendPercentSymbolForLikeOperator(criteriaOperator,false);
                _sqlCommand.AppendDbParameter(new SqlParameter(parameterName, myCriterion.Value));
                parameterIndex++;//递增参数索引
            }

            if (myQuery.SubQueries.Count > 0)
            {
                _sqlCommand.AppendStatement(" "+myQuery.Operator.ToString()+" ");
                foreach (Query subQuery in myQuery.SubQueries)
                {
                    SqlServerQueryTranslator myTranslator = new SqlServerQueryTranslator(_sqlCommand, subQuery);
                    myTranslator.RecursiveAppendCondition(ref parameterIndex); // Recursive Call
                }
            }

            _sqlCommand.AppendStatement(")");
        }

        private void AppendPercentSymbolForLikeOperator(CriteriaOperator criteriaOperator,bool isLeftPercentSymbol)
        {
            if (criteriaOperator == CriteriaOperator.Like || criteriaOperator == CriteriaOperator.NotLike)
            {
                if(!isLeftPercentSymbol)
                    _sqlCommand.AppendStatement("+");
                _sqlCommand.AppendStatement("'%'");
                if(isLeftPercentSymbol)
                    _sqlCommand.AppendStatement("+");
            }
        }

        public static string TranslateCriteriaOperator(CriteriaOperator criteriaOperator)
        {
            if (criteriaOperator == CriteriaOperator.Equal)
                return "=";
            if (criteriaOperator == CriteriaOperator.NotEqual)
                return "<>";
            if (criteriaOperator == CriteriaOperator.GreaterThan)
                return ">";
            if (criteriaOperator == CriteriaOperator.GreaterThanOrEqual)
                return ">=";
            if (criteriaOperator == CriteriaOperator.LesserThan)
                return "<";
            if (criteriaOperator == CriteriaOperator.LesserThanOrEqual)
                return "<=";
            if (criteriaOperator == CriteriaOperator.Like)
                return " LIKE ";
            if (criteriaOperator == CriteriaOperator.NotLike)
                return " NOT LIKE ";
            if (criteriaOperator == CriteriaOperator.IsNull)
                return " IS NULL ";
            if (criteriaOperator == CriteriaOperator.IsNotNull)
                return " IS NOT NULL ";

            throw new ArgumentException("CriteriaOperator not supported", "criteriaOperator");
        }
    }
}