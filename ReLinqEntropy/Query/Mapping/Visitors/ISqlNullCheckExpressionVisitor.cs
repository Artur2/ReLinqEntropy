using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public interface ISqlNullCheckExpressionVisitor
    {
        Expression VisitSqlIsNotNull(SqlIsNotNullExpression sqlIsNotNullExpression);
        Expression VisitSqlIsNull(SqlIsNullExpression sqlIsNullExpression);
    }
}
