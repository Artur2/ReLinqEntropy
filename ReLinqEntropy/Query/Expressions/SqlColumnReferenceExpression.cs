using ReLinqEntropy.Query.Mapping;
using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlColumnReferenceExpression : SqlColumnExpression
    {
        private readonly SqlEntityExpression _referencedEntity;

        public SqlColumnReferenceExpression(Type type, string tableAlias, string referencedColumnName, bool isPrimaryKey, SqlEntityExpression referencedEntity)
            : base(type, tableAlias, referencedColumnName, isPrimaryKey)
        {
            _referencedEntity = referencedEntity;
        }

        public SqlEntityExpression ReferencedEntity => _referencedEntity;

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlColumnExpressionVisitor specificSqlColumnExpression)
            {
                return specificSqlColumnExpression.VisitSqlColumnReference(this);
            }

            return base.Accept(visitor);
        }

        public override SqlColumnExpression Update(Type type, string tableAlias, string columnName, bool isPrimaryKey) =>
            new SqlColumnReferenceExpression(type, tableAlias, columnName, isPrimaryKey, _referencedEntity);

        public override string ToString() => $"[{OwningTableAlias}].[{ReferencedEntity?.Name}{ColumnName}] (REF)";
    }
}
