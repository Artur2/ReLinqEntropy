using System;
using System.Linq.Expressions;

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

        // TODO: Need to end
    }
}
