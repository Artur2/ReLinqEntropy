using ReLinqEntropy.Query.Expressions;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlTableReferenceExpressionVisitor
    {
        Expression VisitSqlTableReference(SqlTableReferenceExpression expression);
    }
}
