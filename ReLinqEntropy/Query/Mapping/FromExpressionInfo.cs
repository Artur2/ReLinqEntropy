using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses;

namespace ReLinqEntropy.Query.Mapping
{
    public class FromExpressionInfo
    {
        private readonly SqlTable _sqlTable;
        private readonly ReadOnlyCollection<Ordering> _extractedOrderings;
        private readonly Expression _itemSelector;
        private readonly Expression _whereCondition;

        public FromExpressionInfo(SqlTable sqlTable, Ordering[] extractedOrderings, Expression itemSelector, Expression whereCondition)
        {
            _sqlTable = sqlTable;
            _extractedOrderings = Array.AsReadOnly(extractedOrderings);
            _itemSelector = itemSelector;
            _whereCondition = whereCondition;
        }

        public SqlTable SqlTable => _sqlTable;

        public ReadOnlyCollection<Ordering> ExtractedOrderings => _extractedOrderings;

        public Expression ItemSelector => _itemSelector;

        public Expression WhereCondition => _whereCondition;
    }
}
