using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlLiteralExpression : Expression
    {
        private readonly Type _type;

        private readonly object _value;

        public SqlLiteralExpression(int value, bool nullable = false)
        : this(value, nullable ? typeof(int?) : typeof(int))
        {
        }

        public SqlLiteralExpression(long value, bool nullable = false)
          : this(value, nullable ? typeof(long?) : typeof(long))
        {
        }

        public SqlLiteralExpression(string value)
          : this(value, typeof(string))
        {
        }

        public SqlLiteralExpression(double value, bool nullable = false)
          : this(value, nullable ? typeof(double?) : typeof(double))
        {
        }

        private SqlLiteralExpression(object value, Type type)
        {
            _type = type;
            _value = value;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public object Value => _value;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor) => this;

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlLiteral(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString()
            => Value is string ? "\"" + Value + "\"" : Value.ToString();
    }
}
