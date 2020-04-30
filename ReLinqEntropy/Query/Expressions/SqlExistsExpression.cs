using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlExistsExpression : Expression
    {
        private readonly Expression _expression;

        public SqlExistsExpression(Expression expression) => _expression = expression;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        public Expression Expression => _expression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newExpression = expressionVisitor.Visit(_expression);
            if (newExpression != _expression)
            {
                return new SqlExistsExpression(newExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlExistsExpressionVisitor sqlExistsExpressionVisitor && sqlExistsExpressionVisitor != null)
            {
                return sqlExistsExpressionVisitor.VisitSqlExists(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString() => $"EXISTS({_expression})";
    }

}
