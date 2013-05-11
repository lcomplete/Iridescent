namespace Iridescent.Data.QueryModel
{
    /// <summary>
    /// Represents an order imposed upon a result set. 
    /// </summary>
    public class OrderClause
    {
        private string propertyName;
        private OrderClauseCriteria criterion;

        /// <summary>
        /// Initializes a new instance of the Idiorm.OrderClause class.
        /// </summary>
        /// <param name="propertyName">The name of the property to order for.</param>
        /// <param name="criteria">The operator of the new Idiorm.OrderClause object.</param>
        public OrderClause(string propertyName, OrderClauseCriteria criteria)
        {
            this.propertyName = propertyName;
            this.criterion = criteria;
        }

        /// <summary>
        /// Gets the name of the property to map
        /// </summary>
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
        }

        /// <summary>
        /// Gets the ordering criterion of the clause
        /// </summary>
        public OrderClauseCriteria Criterion
        {
            get
            {
                return criterion;
            }
        }

        /// <summary>
        /// Specifies the criterion of a Idiorm.OrderClause
        /// </summary>
        public enum OrderClauseCriteria
        {
            /// <summary>
            /// An operator that represents an "ascending" criterion.
            /// </summary>
            Ascending,
            /// <summary>
            /// An operator that represents a "descending" criterion.
            /// </summary>
            Descending
        }
    }
}
