using System;
using ReLinqEntropy.Query.Mapping;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlColumnDefinitionExpression : SqlColumnExpression
    {
        public SqlColumnDefinitionExpression(Type type, string owningTableAlias, string columnName, bool isPrimaryKey) : base(type, owningTableAlias, columnName, isPrimaryKey)
        {
        }

        public override SqlColumnExpression Update(Type type, string owningTableAlias, string columnName, bool isPrimaryKey)
        {
            throw new NotImplementedException();
        }
    }
}