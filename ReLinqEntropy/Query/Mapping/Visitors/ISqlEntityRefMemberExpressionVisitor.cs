using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlEntityRefMemberExpressionVisitor
    {
        Expression VisitSqlEntityRefMember(SqlEntityRefMemberExpression expression);
    }
}
