namespace ReLinqEntropy.Query.Mapping.Statements
{
    public enum SetOperationsMode
    {
        /// <summary>
        /// The <see cref="SqlStatement"/> has <see cref="SqlStatement.SetOperationCombinedStatements"/>. 
        /// <see cref="SqlGeneratingOuterSelectExpressionVisitor"/> will therefore not allow anything that could cause inconsistent in-memory projections,
        /// such as in-memory method calls.
        /// </summary>
        StatementIsSetCombined,
        /// <summary>
        /// The <see cref="SqlStatement"/> has no <see cref="SqlStatement.SetOperationCombinedStatements"/>. 
        /// <see cref="SqlGeneratingOuterSelectExpressionVisitor"/> will therefore support more features.
        /// </summary>
        StatementIsNotSetCombined
    }
}
