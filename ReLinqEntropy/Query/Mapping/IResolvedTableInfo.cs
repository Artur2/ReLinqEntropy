using Remotion.Linq;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping
{
    public interface IResolvedTableInfo : ITableInfo
    {
        string TableAlias { get; }

        Expression ResolveReference(SqlTable sqlTable, IMappingResolver mappingResolver,
            IMappingResolutionContext mappingResolutionContext, UniqueIdentifierGenerator uniqueIdentifierGenerator);
    }
}