using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlEntityDefinitionExpression : SqlEntityExpression
    {
        private readonly ReadOnlyCollection<SqlColumnExpression> _columns;

        public SqlEntityDefinitionExpression(
            Type entityType,
            string tableAlias,
            string entityName,
            Func<SqlEntityExpression, Expression> identityExpressionGenerator,
            params SqlColumnExpression[] projectionColumns) : base(entityType, tableAlias, entityName,
            identityExpressionGenerator) => _columns = Array.AsReadOnly(projectionColumns);

        public override ReadOnlyCollection<SqlColumnExpression> Columns => _columns;

        public override SqlColumnExpression GetColumn(Type type, string columnName, bool isPrimaryKeyColumn)
            => new SqlColumnDefinitionExpression(type, TableAlias, columnName, isPrimaryKeyColumn);

        public override SqlEntityExpression CreateReference(string newTableAlias, Type newType)
            => new SqlEntityReferenceExpression(newType, newTableAlias, null, this);

        public override SqlEntityExpression Update(Type itemType, string tableAlias, string entityName)
            => new SqlEntityDefinitionExpression(itemType, tableAlias, entityName, IdentityExpressionGenerator,
                Columns.ToArray());

        public override string ToString() => $"[{(Name != null ? $"AS [{Name}]" : string.Empty)}] {TableAlias}";

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newColumns = visitor.VisitAndConvert(Columns, "VisitChildren");
            return newColumns != Columns
                ? new SqlEntityDefinitionExpression(Type, TableAlias, null, IdentityExpressionGenerator)
                : this;
        }
    }
}