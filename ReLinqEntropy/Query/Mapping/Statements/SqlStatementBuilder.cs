using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace ReLinqEntropy.Query.Mapping.Statements
{
    public class SqlStatementBuilder
    {
        public IStreamedDataInfo DataInfo { get; set; }

        public bool IsDistinctQuery { get; set; }

        public Expression TopExpression { get; set; }

        public Expression SelectProjection { get; set; }

        public List<SqlTableConcrete> SqlTables { get; set; }

        public Expression WhereCondition { get; set; }

        public Expression GroupByExpression { get; set; }

        public List<Ordering> Orderings { get; set; }

        public Expression RowNumberSelector { get; set; }

        public Expression CurrentRowNumberOffset { get; set; }

        public List<SetOperationCombinedStatement> SetOperationCombinedStatements { get; set; }

        public SqlStatementBuilder() => Reset();

        public SqlStatementBuilder(SqlStatement sqlStatement)
        {
            DataInfo = sqlStatement.DataInfo;
            SelectProjection = sqlStatement.SelectProjection;
            WhereCondition = sqlStatement.WhereCondition;
            IsDistinctQuery = sqlStatement.IsDistinctQuery;
            TopExpression = sqlStatement.TopExpression;
            GroupByExpression = sqlStatement.GroupByExpression;
            RowNumberSelector = sqlStatement.RowNumberSelector;
            CurrentRowNumberOffset = sqlStatement.CurrentRowNumberOffset;

            SqlTables = new List<SqlTableConcrete>(sqlStatement.SqlTables);
            Orderings = new List<Ordering>(sqlStatement.Orderings);
            SetOperationCombinedStatements =
                new List<SetOperationCombinedStatement>(sqlStatement.SetOperationCombinedStatements);
        }

        public SqlStatement GetSqlStatement()
        {
            if (DataInfo == null)
            {
                throw new InvalidOperationException("A DataInfo must be set before the SqlStatement can be retrieved");
            }

            if (SelectProjection == null)
            {
                throw new InvalidOperationException(
                    "A SelectProjection must be set before the SqlStatement can be retrieved.");
            }

            return new SqlStatement(
                DataInfo,
                SelectProjection,
                SqlTables,
                WhereCondition,
                GroupByExpression,
                Orderings,
                TopExpression,
                IsDistinctQuery,
                RowNumberSelector,
                CurrentRowNumberOffset,
                SetOperationCombinedStatements);
        }

        public void AddWhereCondition(Expression translatedExpression)
        {
            if (WhereCondition != null)
            {
                WhereCondition = Expression.AndAlso(WhereCondition, translatedExpression);
            }
            else
            {
                WhereCondition = translatedExpression;
            }
        }

        public SqlStatement GetStatementAndResetBuilder()
        {
            var subSqlStatement = GetSqlStatement();

            Reset();

            return subSqlStatement;
        }

        public void RecalculateDataInfo(Expression previousSelectProjection)
        {
            if (SelectProjection.Type != previousSelectProjection.Type)
            {
                var streamedSequenceInfo = DataInfo as StreamedSequenceInfo;
                if (streamedSequenceInfo != null)
                {
                    DataInfo = new StreamedSequenceInfo(typeof(IQueryable<>).MakeGenericType(SelectProjection.Type),
                        SelectProjection);
                }

                var streamedSingleValueInfo = DataInfo as StreamedSingleValueInfo;
                if (streamedSingleValueInfo != null)
                {
                    DataInfo = new StreamedSingleValueInfo(SelectProjection.Type,
                        streamedSingleValueInfo.ReturnDefaultWhenEmpty);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("SELECT ");
            if (IsDistinctQuery)
            {
                sb.Append("DISTINCT ");
            }

            if (TopExpression != null)
            {
                sb.Append("TOP (").Append(TopExpression).Append(") ");
            }

            if (SelectProjection != null)
            {
                sb.Append(SelectProjection);
            }

            if (SqlTables.Count > 0)
            {
                sb.Append(" FROM ");
                sb.Append(SqlTables.First());
                SqlTables.Skip(1).Aggregate(sb, (builder, table) => builder.Append(", ").Append(table));
            }

            if (WhereCondition != null)
            {
                sb.Append(" WHERE ").Append(WhereCondition);
            }

            if (GroupByExpression != null)
            {
                sb.Append(" GROUP BY ").Append(GroupByExpression);
            }

            if (Orderings.Count > 0)
            {
                sb.Append(" ORDER BY ");
                Orderings.Aggregate(
                    sb,
                    (builder, ordering) => builder
                        .Append(ordering.Expression)
                        .Append(" ")
                        .Append(ordering.OrderingDirection.ToString().ToUpper()));
            }

            foreach (var combinedStatement in SetOperationCombinedStatements)
            {
                sb
                    .Append(" ")
                    .Append(combinedStatement.SetOperation.ToString().ToUpper())
                    .Append(" (")
                    .Append(combinedStatement.SqlStatement)
                    .Append(")");
            }

            return sb.ToString();
        }


        private void Reset()
        {
            DataInfo = null;
            SelectProjection = null;
            WhereCondition = null;
            IsDistinctQuery = false;
            TopExpression = null;
            GroupByExpression = null;
            RowNumberSelector = null;
            CurrentRowNumberOffset = null;

            SqlTables = new List<SqlTableConcrete>();
            Orderings = new List<Ordering>();
            SetOperationCombinedStatements = new List<SetOperationCombinedStatement>();
        }
    }
}