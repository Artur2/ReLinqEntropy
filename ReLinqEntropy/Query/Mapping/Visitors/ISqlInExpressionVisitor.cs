using System.Linq.Expressions;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlInExpressionVisitor
    {
        Expression VisitSqlIn(SqlInExpression expression);
    }
}
