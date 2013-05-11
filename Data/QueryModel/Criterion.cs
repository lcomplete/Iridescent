namespace Iridescent.Data.QueryModel
{
    public class Criterion
    {
        private string propertyName;
        private object value;
        private CriteriaOperator @operator;

        /// <summary>
        /// Initializes a new instance of the Idiorm.Criterion class.
        /// </summary>
        public Criterion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Idiorm.Criterion class that uses the property name, the value and the operator of the new Idiorm.Criterion.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <param name="operator">The operator of the new Idiorm.Criterion object.</param>
        /// <param name="value">The value of the new Idiorm.Criterion object.</param>
        public Criterion(string propertyName, CriteriaOperator @operator, object value)
            : this()
        {
            this.propertyName = propertyName;
            this.value = value;
            this.@operator = @operator;
        }

        /// <summary>
        /// Gets or sets the name of the property to be used by the criterion
        /// </summary>
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                propertyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the criterion
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator of the criterion
        /// </summary>
        public CriteriaOperator Operator
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
    }
}
