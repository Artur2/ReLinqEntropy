using ReLinqEntropy.Query.Mapping;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlEntityReferenceExpression : SqlEntityExpression
    {
        private readonly ReadOnlyCollection<SqlColumnExpression> _columns;
        private readonly SqlEntityExpression _referencedEntity;

        public SqlEntityReferenceExpression(Type itemType, string tableAlias, string entityName, SqlEntityExpression referencedEntity)
            : base(itemType, tableAlias, entityName, referencedEntity.IdentityExpressionGenerator)
        {
            _referencedEntity = referencedEntity;
            _columns = Array.AsReadOnly(referencedEntity.Columns.Select(c => GetColumn(c.Type, c.ColumnName, c.IsPrimaryKey)).ToArray());
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

        public override ReadOnlyCollection<SqlColumnExpression> Columns => _columns;

        public SqlEntityExpression ReferencedEntity => _referencedEntity;

        public override SqlColumnExpression GetColumn(Type type, string columnName, bool isPrimaryKeyColumn)
            => new SqlColumnReferenceExpression(type, TableAlias, columnName, isPrimaryKeyColumn, _referencedEntity);

        public override SqlEntityExpression CreateReference(string newTableAlias, Type newType)
            => new SqlEntityReferenceExpression(newType, newTableAlias, null, this);

        public override SqlEntityExpression Update(Type itemType, string tableAlias, string entityName)
            => new SqlEntityReferenceExpression(itemType, tableAlias, entityName, _referencedEntity);

        public override string ToString() => $"[{TableAlias}][{_referencedEntity?.Name}] (ENTITY-REF)";
    }
}