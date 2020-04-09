using ReLinqEntropy.Query.Expressions;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlSubStatementVisitor
    {
        Expression VisitSqlSubStatement(SqlSubStatementExpression expression);
    }
}
