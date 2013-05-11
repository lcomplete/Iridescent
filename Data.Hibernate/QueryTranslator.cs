using System;
using Iridescent.Data.QueryModel;
using NHibernate;
using NHibernate.Criterion;

namespace Iridescent.Data.Hibernate
{
    internal class QueryTranslator
    {
        private ICriteria _criteria;
        private Query _query;
        
        public QueryTranslator(ICriteria criteria, Query query)
        {
            _criteria = criteria;
            _query = query;
        }
        
        public void Execute()
        {
            Query myQuery = this._query;

            foreach (OrderClause clause in myQuery.OrderClauses)
            {
                _criteria.AddOrder(new NHibernate.Criterion.Order(clause.PropertyName, clause.Criterion == OrderClause.OrderClauseCriteria.Ascending ? true : false));
            }

            foreach (Criterion myCriterion in myQuery.Criteria)
            {
                ICriterion criterion = null;
                if (myCriterion.Operator == CriteriaOperator.Equal)
                    criterion = Expression.Eq(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.NotEqual)
                    criterion = Expression.Not(Expression.Eq(myCriterion.PropertyName, myCriterion.Value));
                else if (myCriterion.Operator == CriteriaOperator.GreaterThan)
                    criterion = Expression.Gt(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.GreaterThanOrEqual)
                    criterion = Expression.Ge(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.LesserThan)
                    criterion = Expression.Lt(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.LesserThanOrEqual)
                    criterion = Expression.Le(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.Like)
                    criterion = Expression.Like(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.NotLike)
                    criterion = Expression.Not(Expression.Like(myCriterion.PropertyName, myCriterion.Value));
                else if (myCriterion.Operator == CriteriaOperator.IsNull)
                    criterion = Expression.IsNull(myCriterion.PropertyName);
                else if (myCriterion.Operator == CriteriaOperator.IsNotNull)
                    criterion = Expression.IsNotNull(myCriterion.PropertyName);
                else
                    throw new ArgumentException("operator", "CriteriaOperator not supported in NHibernate Provider");

                if (_query.Operator == QueryOperator.And)
                    _criteria.Add(Expression.Conjunction().Add(criterion));
                else if (_query.Operator == QueryOperator.Or)
                    _criteria.Add(Expression.Disjunction().Add(criterion));
            }

            foreach (Query subQuery in myQuery.SubQueries)
            {
                QueryTranslator myTranslator = new QueryTranslator(_criteria, _query);
                myTranslator.Execute(); // Recursive Call
            }
        }
    }
}
