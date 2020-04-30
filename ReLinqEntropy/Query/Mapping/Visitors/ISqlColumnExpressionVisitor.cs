using System.Linq.Expressions;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlColumnExpressionVisitor
    {
        Expression VisitSqlColumnDefinition(SqlColumnDefinitionExpression expression);

        Expression VisitSqlColumnReference(SqlColumnReferenceExpression expression);
    }
}
