using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlCollectionExpression : Expression
    {
        private readonly Type _type;
        private readonly ReadOnlyCollection<Expression> _items;

        public SqlCollectionExpression(Type type, IEnumerable<Expression> items)
        {
            _type = type;
            _items = items.ToList().AsReadOnly();
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public ReadOnlyCollection<Expression> Items => _items;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newItems = expressionVisitor.VisitAndConvert(_items, "SqlCollectionExpression.VisitChildren");
            if (newItems != _items)
            {
                return new SqlCollectionExpression(_type, newItems);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlCollectionExpressionVisitor sqlCollectionExpressionVisitor && sqlCollectionExpressionVisitor != null)
            {
                return sqlCollectionExpressionVisitor.VisitSqlCollection(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString() => $"({string.Join(", ", _items.Select(e => e.ToString()))})";
    }
}
