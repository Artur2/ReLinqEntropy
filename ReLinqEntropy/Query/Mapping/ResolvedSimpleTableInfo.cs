using System;
using System.Linq.Expressions;
using Remotion.Linq;

namespace ReLinqEntropy.Query.Mapping
{
    public class ResolvedSimpleTableInfo : IResolvedTableInfo
    {
        private readonly Type _itemType;
        private readonly string _tableName;
        private readonly string _tableAlias;

        public ResolvedSimpleTableInfo(Type itemType, string tableName, string tableAlias)
        {
            _itemType = itemType;
            _tableName = tableName;
            _tableAlias = tableAlias;
        }

        public Type ItemType => _itemType;
        
        public IJoinInfo Accept(IJoinInfoVisitor joinInfoVisitor)
        {
            throw new NotImplementedException();
        }

        public ResolvedJoinInfo GetResolvedJoinInfo()
        {
            throw new NotImplementedException();
        }

        public string TableAlias => _tableAlias;

        public Expression ResolveReference(SqlTable sqlTable, IMappingResolver mappingResolver,
            IMappingResolutionContext mappingResolutionContext, UniqueIdentifierGenerator uniqueIdentifierGenerator)
        {
            throw new NotImplementedException();
        }
    }
}