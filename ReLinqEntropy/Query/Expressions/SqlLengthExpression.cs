using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlLengthExpression : Expression
    {
        private readonly Expression _expression;

        public SqlLengthExpression(Expression expression)
        {
            if (expression.Type != typeof(string) && expression.Type != typeof(char))
            {
                throw new ArgumentException("Allowed only string or char type", nameof(expression));
            }

            _expression = expression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(int);

        public Expression Expression => _expression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newExpression = expressionVisitor.Visit(_expression);

            if (newExpression != _expression)
            {
                return new SqlLengthExpression(newExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlLength(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString() => $"LEN({_expression})";
    }
}
