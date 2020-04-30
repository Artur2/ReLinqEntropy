using System.Linq.Expressions;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlNullCheckExpressionVisitor
    {
        Expression VisitSqlIsNotNull(SqlIsNotNullExpression sqlIsNotNullExpression);
        Expression VisitSqlIsNull(SqlIsNullExpression sqlIsNullExpression);
    }
}
