using ReLinqEntropy.Query.Expressions;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface IResolvedSqlExpressionVisitor
    {
        Expression VisitSqlEntity(SqlEntityExpression expression);
        Expression VisitSqlColumn(SqlColumnExpression expression);
        Expression VisitSqlEntityConstant(SqlEntityConstantExpression expression);
    }
}