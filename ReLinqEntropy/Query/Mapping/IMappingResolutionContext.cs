using ReLinqEntropy.Query.Expressions;
using ReLinqEntropy.Query.Mapping.Statements;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping
{
    public interface IMappingResolutionContext
    {
        void AddSqlEntityMapping(SqlEntityExpression sqlEntityExpression, SqlTable sqlTable);

        void AddGroupReferenceMapping(SqlGroupingSelectExpression sqlGroupingSelectExpression, SqlTable sqlTable);

        SqlTable GetSqlTableForEntityExpression(SqlEntityExpression sqlEntityExpression);

        SqlTable GetReferencedGroupSource(SqlGroupingSelectExpression groupingSelectExpression);

        SqlEntityExpression UpdateEntityAndAddMapping(SqlEntityExpression entityExpression, Type itemType, string tableAlias, string newName);

        SqlGroupingSelectExpression UpdateGroupingSelectAndAddMapping(
            SqlGroupingSelectExpression expression, Expression newKey, Expression newElement, IEnumerable<Expression> aggregations);

        void AddSqlTable(SqlTable sqlTable, SqlStatementBuilder sqlStatementBuilder);

        Expression RemoveNamesAndUpdateMapping(Expression expression);
    }
}