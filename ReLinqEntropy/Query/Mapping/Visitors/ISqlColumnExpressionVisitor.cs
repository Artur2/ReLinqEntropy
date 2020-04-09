using ReLinqEntropy.Query.Expressions;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlColumnExpressionVisitor
    {
        Expression VisitSqlColumnDefinition(SqlColumnDefinitionExpression expression);

        Expression VisitSqlColumnReference(SqlColumnReferenceExpression expression);
    }
}
