﻿using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlIsNullExpression : Expression
    {
        private readonly Expression _expression;

        public SqlIsNullExpression(Expression expression) => _expression = expression;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        public Expression Expression => _expression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newExpression = expressionVisitor.Visit(_expression);

            if (newExpression != _expression)
            {
                return new SqlIsNullExpression(newExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlNullCheckExpressionVisitor sqlNullCheckExpressionVisitor && sqlNullCheckExpressionVisitor != null)
            {
                return sqlNullCheckExpressionVisitor.VisitSqlIsNull(this);
            }

            return base.Accept(expressionVisitor);
        }
    }
}
