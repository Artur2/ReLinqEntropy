using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Statements;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlSubStatementExpression : Expression
    {
        private readonly SqlStatement _sqlStatement;

        public SqlSubStatementExpression (SqlStatement sqlStatement)
        {
            _sqlStatement = sqlStatement;
        }
    }
}