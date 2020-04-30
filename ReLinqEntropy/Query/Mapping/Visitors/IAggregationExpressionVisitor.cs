using System.Linq.Expressions;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface IAggregationExpressionVisitor
    {
        Expression VisitAggregation(AggregationExpression expression);
    }
}
