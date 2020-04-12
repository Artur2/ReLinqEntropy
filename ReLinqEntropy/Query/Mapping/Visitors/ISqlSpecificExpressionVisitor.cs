using ReLinqEntropy.Query.Expressions;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlSpecificExpressionVisitor
    {
        Expression VisitSqlLiteral(SqlLiteralExpression expression);
        Expression VisitSqlFunction(SqlFunctionExpression expression);
        Expression VisitSqlConvert(SqlConvertExpression expression);
        Expression VisitSqlRowNumber(SqlRowNumberExpression expression);
        Expression VisitSqlLike(SqlLikeExpression expression);
        Expression VisitSqlLength(SqlLengthExpression expression);
        Expression VisitSqlCase(SqlCaseExpression expression);
    }
}
