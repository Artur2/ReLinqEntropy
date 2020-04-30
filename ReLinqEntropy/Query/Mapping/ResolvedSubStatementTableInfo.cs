using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Statements;
using Remotion.Linq;

namespace ReLinqEntropy.Query.Mapping
{
    public class ResolvedSubStatementTableInfo : IResolvedTableInfo
    {
        private readonly Type _itemType;
        private readonly string _tableAlias;
        private readonly SqlStatement _sqlStatement;

        public ResolvedSubStatementTableInfo(string tableAlias, SqlStatement sqlStatement) => _tableAlias = tableAlias;

        public SqlStatement SqlStatement => _sqlStatement;

        public Type ItemType => _itemType;

        public string TableAlias => _tableAlias;

        public IResolvedTableInfo GetResolvedTableInfo() => this;

        public ITableInfo Accept(ITableInfoVisitor tableInfoVisitor) => tableInfoVisitor.VisitSubStatementTableInfo(this);

        public Expression ResolveReference(SqlTable sqlTable, IMappingResolver mappingResolver,
            IMappingResolutionContext mappingResolutionContext, UniqueIdentifierGenerator uniqueIdentifierGenerator)
        {
            var selectProjection = SqlStatement.SelectProjection;
            return SubStatementReferenceResolver.ResolveSubStatementReferenceExpression(selectProjection, this, sqlTable, mappingResolutionContext);
        }

        public override string ToString() => $"({SqlStatement}) [{TableAlias}]";
    }
}