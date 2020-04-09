using ReLinqEntropy.Query.Mapping.Statements;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlSubStatementExpression : Expression
    {
        private readonly SqlStatement _sqlStatement;

        public SqlSubStatementExpression(SqlStatement sqlStatement)
        {
            _sqlStatement = sqlStatement;
        }
    }
}