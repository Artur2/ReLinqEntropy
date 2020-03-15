using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReLinqEntropy.Internal;
using ReLinqEntropy.Query.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace ReLinqEntropy.Query.Mapping.Statements
{
    public class SqlStatement
    {
        private readonly IStreamedDataInfo _dataInfo;
        private readonly Expression _selectProjection;
        private readonly SqlTableConcrete[] _sqlTables;
        private readonly Expression _groupByExpression;
        private readonly Ordering[] _orderings;
        private readonly Expression _whereCondition;
        private readonly Expression _topExpression;
        private readonly bool _isDistinctQuery;
        private readonly Expression _rowNumberSelector;
        private readonly Expression _currentRowNumberOffset;
        private readonly SetOperationCombinedStatement[] _setOperationCombinedStatements;

        public SqlStatement(
            IStreamedDataInfo dataInfo,
            Expression selectProjection,
            IEnumerable<SqlTableConcrete> sqlTables,
            Expression whereCondition,
            Expression groupCondition,
            IEnumerable<Ordering> orderings,
            Expression topExpression,
            bool isDistinctQuery,
            Expression rowNumberSelector,
            Expression currentRowNumberOffset,
            IEnumerable<SetOperationCombinedStatement> setOperationCombinedStatements)
        {
            if (whereCondition != null && whereCondition.Type != typeof(bool))
            {
                throw new ArgumentException(ExceptionMessageConstants.WrongArgumentType);
            }

            _dataInfo = dataInfo;
            _selectProjection = selectProjection;
            _sqlTables = sqlTables.ToArray();
            _orderings = orderings.ToArray();
            _whereCondition = whereCondition;
            _topExpression = topExpression;
            _isDistinctQuery = isDistinctQuery;
            _rowNumberSelector = rowNumberSelector;
            _currentRowNumberOffset = currentRowNumberOffset;
            _setOperationCombinedStatements = setOperationCombinedStatements.ToArray();
        }

        public IStreamedDataInfo DataInfo => _dataInfo;

        public bool IsDistinctQuery => _isDistinctQuery;

        public Expression TopExpression => _topExpression;

        public Expression SelectProjection => _selectProjection;

        public IReadOnlyCollection<SqlTableConcrete> SqlTables => Array.AsReadOnly(_sqlTables);

        public Expression WhereCondition => _whereCondition;

        public Expression GroupByExpression => _groupByExpression;

        public IReadOnlyCollection<Ordering> Orderings => Array.AsReadOnly(_orderings);

        public Expression RowNumberSelector => _rowNumberSelector;

        public Expression CurrentRowNumberOffset => _currentRowNumberOffset;

        public IReadOnlyCollection<SetOperationCombinedStatement> SetOperationCombinedStatements =>
            Array.AsReadOnly(_setOperationCombinedStatements);

        public Expression CreateExpression() => SqlTables.Count == 0 && !IsDistinctQuery
            ? SelectProjection
            : new SqlSubStatementExpression(this);
        
        public override bool Equals (object obj)
        {
            var statement = obj as SqlStatement;
            if (statement == null)
                return false;

            return (_dataInfo.Equals (statement._dataInfo))
                   && (_selectProjection == statement._selectProjection)
                   && (_whereCondition == statement._whereCondition)
                   && (_topExpression == statement._topExpression)
                   && (_isDistinctQuery == statement._isDistinctQuery)
                   && (_rowNumberSelector == statement._rowNumberSelector)
                   && (_currentRowNumberOffset == statement._currentRowNumberOffset)
                   && (_groupByExpression == statement._groupByExpression)
                   // Note: These items are all compared by reference, which is okay because the visitors take care to reuse the objects if their contents
                   // don't change.
                   && (_sqlTables.SequenceEqual (statement._sqlTables))
                   && (_orderings.SequenceEqual (statement._orderings))
                   && (_setOperationCombinedStatements.SequenceEqual(statement.SetOperationCombinedStatements));
        }

        public override int GetHashCode ()
        {
            return EqualityUtility.GetRotatedHashCode(
                       _dataInfo,
                       _selectProjection,
                       _whereCondition,
                       _topExpression,
                       _isDistinctQuery,
                       _rowNumberSelector,
                       _currentRowNumberOffset,
                       _groupByExpression)
                   ^ EqualityUtility.GetRotatedHashCode(_sqlTables)
                   ^ EqualityUtility.GetRotatedHashCode(_orderings)
                   ^ EqualityUtility.GetRotatedHashCode(_setOperationCombinedStatements);
        }

        public override string ToString ()
        {
            // TODO: Add sql statement builder
            throw new NotImplementedException();
        }
    }
}