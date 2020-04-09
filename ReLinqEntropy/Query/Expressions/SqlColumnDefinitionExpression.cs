using ReLinqEntropy.Query.Mapping;
using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlColumnDefinitionExpression : SqlColumnExpression
    {
        public SqlColumnDefinitionExpression(Type type, string owningTableAlias, string columnName, bool isPrimaryKey) : base(type, owningTableAlias, columnName, isPrimaryKey)
        {
        }

        public override SqlColumnExpression Update(Type type, string owningTableAlias, string columnName, bool isPrimaryKey)
            => new SqlColumnDefinitionExpression(type, owningTableAlias, columnName, isPrimaryKey);

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlColumnExpressionVisitor specificSqlColumnExpressionVisitor)
            {
                return specificSqlColumnExpressionVisitor.VisitSqlColumnDefinition(this);
            }

            return base.Accept(visitor);
        }

        public override string ToString() => $"[{OwningTableAlias}].[{ColumnName}]";
    }
}