namespace ReLinqEntropy.Query.Mapping.Statements
{
    public class SetOperationCombinedStatement
    {
        private readonly SqlStatement _sqlStatement;
        private readonly SetOperation _setOperation;

        public SetOperationCombinedStatement(SqlStatement sqlStatement, SetOperation setOperation)
        {
            _sqlStatement = sqlStatement;
            _setOperation = setOperation;
        }

        public SqlStatement SqlStatement => _sqlStatement;

        public SetOperation SetOperation => _setOperation;
    }
}