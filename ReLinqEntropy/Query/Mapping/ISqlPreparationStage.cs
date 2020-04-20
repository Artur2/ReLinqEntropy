using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ReLinqEntropy.Query.Mapping.Contexts;
using ReLinqEntropy.Query.Mapping.Statements;
using Remotion.Linq;

namespace ReLinqEntropy.Query.Mapping
{
    public interface ISqlPreparationStage
    {
        Expression PrepareSelectExpression(Expression expression, ISqlPreparationContext context);
        Expression PrepareWhereExpression(Expression expression, ISqlPreparationContext context);
        Expression PrepareTopExpression(Expression expression, ISqlPreparationContext context);
        Expression PrepareOrderByExpression(Expression expression, ISqlPreparationContext context);
        Expression PrepareResultOperatorItemExpression(Expression expression, ISqlPreparationContext context);

        FromExpressionInfo PrepareFromExpression(
            Expression fromExpression,
            ISqlPreparationContext context,
            Func<ITableInfo, SqlTable> tableGenerator,
            OrderingExtractionPolicy orderingExtractionPolicy);

        SqlStatement PrepareSqlStatement(QueryModel queryModel, ISqlPreparationContext parentContext);
    }
}
