using System.Collections.Generic;

namespace Iridescent.Data.QueryModel
{
    public sealed class Query
    {
        private IList<Criterion> _criteria;
        private QueryOperator @operator;
        private IList<Query> _subQueries = new List<Query>();
        private IList<OrderClause> _orderClauses;

        public Query(IList<Criterion> criteria, IList<OrderClause> orderClauses = null)
        {
            _criteria = criteria ?? new List<Criterion>();
            _orderClauses = orderClauses ?? new List<OrderClause>();
        }

        public Query(IList<OrderClause> orderClauses)
            : this(null, orderClauses)
        {
        }

        public Query(Criterion criterion, OrderClause orderClause = null)
            : this(criterion == null ? null : new List<Criterion> { criterion }, orderClause == null ? null : new List<OrderClause> { orderClause })
        {
        }

        public Query(OrderClause orderClause)
            : this(null, orderClause)
        {
        }

        public Query():this(new List<Criterion>(),new List<OrderClause>())
        {
            
        }

        public IList<Criterion> Criteria
        {
            get
            {
                return _criteria;
            }
        }

        public QueryOperator Operator
        {
            get
            {
                return @operator;
            }
            set
            {
                @operator = value;
            }
        }

        public IList<Query> SubQueries
        {
            get
            {
                return _subQueries;
            }
        }

        public IList<OrderClause> OrderClauses
        {
            get
            {
                return _orderClauses;
            }
        }
    }

}