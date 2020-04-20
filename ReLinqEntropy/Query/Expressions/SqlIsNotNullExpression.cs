using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlIsNotNullExpression : Expression
    {
        private readonly Expression _expression;

        public SqlIsNotNullExpression(Expression expression)
        {
            _expression = expression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        public Expression Expression => _expression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newExpression = expressionVisitor.Visit(_expression);
            if (newExpression != _expression)
            {
                return new SqlIsNotNullExpression(newExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlNullCheckExpressionVisitor sqlNullCheckExpressionVisitor && sqlNullCheckExpressionVisitor != null)
            {
                return sqlNullCheckExpressionVisitor.VisitSqlIsNotNull(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString() => $"{_expression} IS NOT NULL";
    }
}
