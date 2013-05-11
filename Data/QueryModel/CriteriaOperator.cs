namespace Iridescent.Data.QueryModel
{
    /// <summary>
    /// Specifies the operator of a criterion
    /// </summary>
    public enum CriteriaOperator
    {
        /// <summary>
        /// An operator that represents an "equal" criterion.
        /// </summary>
        Equal,
        /// <summary>
        /// An operator that represents a "not equal" criterion.
        /// </summary>
        NotEqual,
        /// <summary>
        /// An operator that represents a "greater than" criterion.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// An operator that represents a "lesser than" criterion.
        /// </summary>
        LesserThan,
        /// <summary>
        /// An operator that represents a "greater than or equal" criterion.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// An operator that represents a "lesser than or equal" criterion.
        /// </summary>
        LesserThanOrEqual,
        /// <summary>
        /// An operator that represents a "like" criterion.
        /// </summary>
        Like,
        /// <summary>
        /// An operator that represents a "not like" criterion.
        /// </summary>
        NotLike,
        /// <summary>
        /// An operator that represents a "is null" criterion.
        /// </summary>
        IsNull,
        /// <summary>
        /// An operator that represents a "is not null" criterion.
        /// </summary>
        IsNotNull
    }
}
