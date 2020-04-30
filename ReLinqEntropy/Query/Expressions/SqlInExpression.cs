using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlInExpression : Expression
    {
        private readonly Expression _leftExpression;
        private readonly Expression _rightExpression;

        public SqlInExpression(Expression leftExpression, Expression rightExpression)
        {
            _leftExpression = leftExpression;
            _rightExpression = rightExpression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        public Expression LeftExpression => _leftExpression;

        public Expression RightExpression => _rightExpression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newLeftExpression = expressionVisitor.Visit(_leftExpression);
            var newRightExpression = expressionVisitor.Visit(_rightExpression);

            if (newLeftExpression != _leftExpression || newRightExpression != _rightExpression)
            {
                return new SqlInExpression(newLeftExpression, newRightExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlInExpressionVisitor specificVisitor && specificVisitor != null)
            {
                return specificVisitor.VisitSqlIn(this);
            }

            return base.Accept(visitor);
        }

        public override string ToString() => $"{_leftExpression} IN {_rightExpression}";
    }
}
