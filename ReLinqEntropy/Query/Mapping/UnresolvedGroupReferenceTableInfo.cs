using System;
using ReLinqEntropy.Query.Expressions;

namespace ReLinqEntropy.Query.Mapping
{
    public class UnresolvedGroupReferenceTableInfo : ITableInfo
    {
        private readonly SqlTable _referencedGroupSource;
        private readonly Type _itemType;

        public UnresolvedGroupReferenceTableInfo(SqlTable referencedGroupSource) => _referencedGroupSource = referencedGroupSource;

        public Type ItemType => _itemType;

        public SqlTable ReferencedGroupSource => _referencedGroupSource;

        public ITableInfo Accept(ITableInfoVisitor tableInfoVisitor)
            => tableInfoVisitor.VisitUnresolvedGroupReferenceTableInfo(this);

        public IResolvedTableInfo GetResolvedTableInfo() => throw new InvalidOperationException("This table has not yet been resolved; call the resolution step first.");

        public override string ToString()
            => $"GROUP-REF-TABLE({new SqlTableReferenceExpression(ReferencedGroupSource)})";
    }
}