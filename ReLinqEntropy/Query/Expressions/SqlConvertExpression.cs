using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlConvertExpression : Expression
    {
        private readonly Type _targetType;
        private readonly Expression _source;

        private static readonly Dictionary<Type, string> _sqlTypeMapping = new Dictionary<Type, string>
        {
            [typeof(string)] = "NVARCHAR(MAX)",
            [typeof(int)] = "INT",
            [typeof(bool)] = "BIT",
            [typeof(long)] = "BIGINT",
            [typeof(char)] = "CHAR",
            [typeof(DateTime)] = "DATETIME",
            [typeof(decimal)] = "DECIMAL",
            [typeof(double)] = "FLOAT",
            [typeof(short)] = "SMALLINT",
            [typeof(Guid)] = "UNIQUEIDENTIFIER"
        };

        public static string GetSqlTypeName(Type type)
        {
            if (_sqlTypeMapping.ContainsKey(type))
                return _sqlTypeMapping[type];

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return GetSqlTypeName(underlyingType);

            return null;
        }

        public SqlConvertExpression(Type targetType, Expression source)
        {
            _targetType = targetType;
            _source = source;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _targetType;

        public Expression Source => _source;

        public string GetSqlTypeName()
        {
            var typename = GetSqlTypeName();
            if (typename == null)
            {
                throw new NotSupportedException($"Type {Type.Name} not supported");
            }

            return typename;
        }

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newSource = expressionVisitor.Visit(_source);

            if (newSource != _source)
            {
                return new SqlConvertExpression(Type, newSource);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlConvert(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString() => $"CONVERT({GetSqlTypeName()}, {_source})";
    }
}
