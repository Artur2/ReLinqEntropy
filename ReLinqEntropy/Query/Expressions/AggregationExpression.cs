using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class AggregationExpression : Expression
    {
        private readonly Type _type;
        private readonly Expression _expression;
        private readonly AggregationModifier _aggregationModifier;

        public AggregationExpression(Type type, Expression expression, AggregationModifier aggregationModifier)
        {
            _type = type;
            _expression = expression;
            _aggregationModifier = aggregationModifier;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public Expression Expression => _expression;

        public AggregationModifier AggregationModifier => _aggregationModifier;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newExpression = expressionVisitor.Visit(_expression);
            if (newExpression != _expression)
            {
                return new AggregationExpression(Type, newExpression, _aggregationModifier);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is IAggregationExpressionVisitor aggregationExpressionVisitor && aggregationExpressionVisitor != null)
                return aggregationExpressionVisitor.VisitAggregation(this);

            return base.Accept(visitor);
        }

        public override string ToString() => $"{_aggregationModifier}({_expression})";
    }

    public enum AggregationModifier
    {
        Count,
        Sum,
        Average,
        Min,
        Max
    }
}
