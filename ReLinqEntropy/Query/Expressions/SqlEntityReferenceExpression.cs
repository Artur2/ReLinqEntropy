using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlEntityReferenceExpression : SqlEntityExpression
    {
        public SqlEntityReferenceExpression (Type itemType, string tableAlias, string entityName, SqlEntityExpression referencedEntity) 
            : base(itemType, tableAlias, entityName, referencedEntity.IdentityExpressionGenerator)
        {
        }

        public override SqlColumnExpression GetColumn(Type type, string columnName, bool isPrimaryKeyColumn)
        {
            throw new NotImplementedException();
        }

        public override SqlEntityExpression CreateReference(string newTableAlias, Type newType)
        {
            throw new NotImplementedException();
        }

        public override SqlEntityExpression Update(Type itemType, string tableAlias, string entityName)
        {
            throw new NotImplementedException();
        }
    }
}