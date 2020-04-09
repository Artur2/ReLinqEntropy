using ReLinqEntropy.Query.Expressions;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlGroupingSelectExpressionVisitor
    {
        Expression VisitSqlGroupingSelect(SqlGroupingSelectExpression expression);
    }
}
