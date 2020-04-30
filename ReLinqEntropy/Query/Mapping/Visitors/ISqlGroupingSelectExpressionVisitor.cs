using System.Linq.Expressions;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlGroupingSelectExpressionVisitor
    {
        Expression VisitSqlGroupingSelect(SqlGroupingSelectExpression expression);
    }
}
