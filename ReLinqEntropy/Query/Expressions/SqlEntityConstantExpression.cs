using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlEntityConstantExpression : Expression
    {
        private readonly Type _type;
        private readonly object _value;
        private readonly Expression _identityExpression;

        public SqlEntityConstantExpression(Type type, object value, Expression identityExpression)
        {
            _type = type;
            _value = value;
            _identityExpression = identityExpression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public object Value => _value;

        public Expression IdentityExpression => _identityExpression;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newPrimaryKeyExpression = visitor.Visit(_identityExpression);
            if (newPrimaryKeyExpression != _identityExpression)
            {
                return new SqlEntityConstantExpression(Type, _value, newPrimaryKeyExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is IResolvedSqlExpressionVisitor specificVisitor)
            {
                return specificVisitor.VisitSqlEntityConstant(this);
            }

            return base.Accept(visitor);
        }

        public override string ToString() => $"Entity({_identityExpression})";
    }
}