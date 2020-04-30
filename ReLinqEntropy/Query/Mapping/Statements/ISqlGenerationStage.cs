using System.Linq.Expressions;
using Remotion.Linq.Clauses;

namespace ReLinqEntropy.Query.Mapping.Statements
{
    public interface ISqlGenerationStage
    {
        void GenerateTextForFromTable(ISqlCommandBuilder commandBuilder, SqlTable table, bool isFirstTable);
        void GenerateTextForSelectExpression(ISqlCommandBuilder commandBuilder, Expression expression);
        void GenerateTextForOuterSelectExpression(ISqlCommandBuilder commandBuilder, Expression expression, SetOperationsMode setOperationsMode);
        void GenerateTextForWhereExpression(ISqlCommandBuilder commandBuilder, Expression expression);
        void GenerateTextForOrderByExpression(ISqlCommandBuilder commandBuilder, Expression expression);
        void GenerateTextForTopExpression(ISqlCommandBuilder commandBuilder, Expression expression);
        void GenerateTextForSqlStatement(ISqlCommandBuilder commandBuilder, SqlStatement sqlStatement);
        void GenerateTextForOuterSqlStatement(ISqlCommandBuilder commandBuilder, SqlStatement sqlStatement);
        void GenerateTextForJoinCondition(ISqlCommandBuilder commandBuilder, Expression expression);
        void GenerateTextForGroupByExpression(ISqlCommandBuilder commandBuilder, Expression expression);
        void GenerateTextForOrdering(ISqlCommandBuilder commandBuilder, Ordering ordering);
    }
}
