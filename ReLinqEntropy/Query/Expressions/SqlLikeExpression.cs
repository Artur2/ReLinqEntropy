using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlLikeExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Expression _escapeExpression;

        public SqlLikeExpression(Expression left, Expression right, Expression escapeExpression)
        {
            _left = left;
            _right = right;
            _escapeExpression = escapeExpression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        public Expression Left => _left;

        public Expression Right => _right;

        public Expression EscapeExpression => _escapeExpression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newLeftExpression = expressionVisitor.Visit(_left);
            var newRightExpression = expressionVisitor.Visit(_right);
            var newEscapeExpression = expressionVisitor.Visit(_escapeExpression);

            if (newLeftExpression != _left || newRightExpression != _right || newEscapeExpression != _escapeExpression)
            {
                return new SqlLikeExpression(newLeftExpression, newRightExpression, newEscapeExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlLike(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString()
            => $"{_left} LIKE {_right} ESCAPE {_escapeExpression}";
    }
}
