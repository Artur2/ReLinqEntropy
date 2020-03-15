using System.Linq.Expressions;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface IResolvedSqlExpressionVisitor
    {
        Expression VisitSqlEntity (SqlEntityExpression expression);
        Expression VisitSqlColumn (SqlColumnExpression expression);
        Expression VisitSqlEntityConstant (SqlEntityConstantExpression expression);
    }
}