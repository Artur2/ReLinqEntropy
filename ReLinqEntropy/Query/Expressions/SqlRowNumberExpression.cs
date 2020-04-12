using ReLinqEntropy.Query.Mapping.Visitors;
using Remotion.Linq.Clauses;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlRowNumberExpression : Expression
    {
        private readonly ReadOnlyCollection<Ordering> _orderings;

        public SqlRowNumberExpression(Ordering[] orderings)
        {
            _orderings = Array.AsReadOnly(orderings);
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(int);

        public ReadOnlyCollection<Ordering> Orderings => _orderings;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newOrderings = Orderings.Select(o =>
            {
                var newExpression = expressionVisitor.Visit(o.Expression);
                return newExpression != o.Expression ? new Ordering(newExpression, o.OrderingDirection) : o;
            }).ToArray();

            if (!newOrderings.SequenceEqual(Orderings))
            {
                return new SqlRowNumberExpression(newOrderings);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlRowNumber(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString()
            => $"ROW_NUMBER() OVER (ORDER BY {string.Join(", ", Orderings.Select(o => o.ToString()))})";
    }
}
