using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping
{
    public abstract class SqlColumnExpression : Expression
    {
        private readonly Type _type;
        private readonly string _owningTableAlias;
        private readonly string _columnName;
        private readonly bool _isPrimaryKey;

        protected SqlColumnExpression(Type type, string owningTableAlias, string columnName, bool isPrimaryKey)
        {
            _type = type;
            _owningTableAlias = owningTableAlias;
            _columnName = columnName;
            _isPrimaryKey = isPrimaryKey;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public string OwningTableAlias => _owningTableAlias;

        public string ColumnName => _columnName;

        public bool IsPrimaryKey => _isPrimaryKey;

        public abstract SqlColumnExpression Update(Type type, string tableAlias, string columnName,
            bool isPrimaryKey);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is IResolvedSqlExpressionVisitor resolvedSqlExpressionVisitor)
            {
                return resolvedSqlExpressionVisitor.VisitSqlColumn(this);
            }

            return base.Accept(visitor);
        }
    }
}