using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping;
using ReLinqEntropy.Query.Mapping.Statements;
using ReLinqEntropy.Query.Mapping.Visitors;
using Remotion.Linq.Clauses.StreamedData;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlSubStatementExpression : Expression
    {
        private readonly SqlStatement _sqlStatement;

        public SqlSubStatementExpression(SqlStatement sqlStatement) => _sqlStatement = sqlStatement;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _sqlStatement.DataInfo.DataType;

        public SqlStatement SqlStatement => _sqlStatement;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor) => this;

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSubStatementVisitor specificVisitor && specificVisitor != null)
            {
                return specificVisitor.VisitSqlSubStatement(this);
            }

            return base.Accept(expressionVisitor);
        }

        public SqlTableConcrete ConvertToSqlTable(string uniqueIdentifier)
        {
            var joinSemantics = CalculateJoinSemantic();

            SqlStatement sequenceStatement;
            if (SqlStatement.DataInfo is StreamedSequenceInfo streamedSequenceInfo)
            {
                sequenceStatement = SqlStatement;
            }
            else
            {
                sequenceStatement = ConvertValueStatementToSequenceStatement();
            }

            var resulvedSubStatementTableInfo = new ResolvedSubStatementTableInfo(uniqueIdentifier, sequenceStatement);

            return new SqlTableConcrete(resulvedSubStatementTableInfo, joinSemantics);
        }

        private SqlStatement ConvertValueStatementToSequenceStatement()
        {
            var newDataInfo = new StreamedSequenceInfo(typeof(IEnumerable<>).MakeGenericType(SqlStatement.DataInfo.DataType), SqlStatement.SelectProjection);

            var adjustedStatementBuilder = new SqlStatementBuilder(SqlStatement) { DataInfo = newDataInfo };
            if (SqlStatement.DataInfo is StreamedSingleValueInfo && SqlStatement.SqlTables.Count != 0)
            {
                // A sub-statement might use a different TopExpression than 1 (or none at all) in order to provoke a SQL error when more than one item is 
                // returned. When we convert the statement to a sequence statement, however, we must ensure that the exact "only 1 value is returned" 
                // semantics is ensured because we can't provoke a SQL error (but instead would return strange result sets).
                adjustedStatementBuilder.TopExpression = new SqlLiteralExpression(1);
            }

            return adjustedStatementBuilder.GetSqlStatement();
        }


        private JoinSemantics CalculateJoinSemantic()
        {
            var dataInfoAsStreamedSingleValueInfo = SqlStatement.DataInfo as StreamedSingleValueInfo;
            if (dataInfoAsStreamedSingleValueInfo != null && dataInfoAsStreamedSingleValueInfo.ReturnDefaultWhenEmpty)
            {
                return JoinSemantics.Left;
            }

            return JoinSemantics.Inner;
        }
    }
}