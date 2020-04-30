using System.Linq.Expressions;
using Remotion.Linq;

namespace ReLinqEntropy.Query.Mapping
{
    public interface IResolvedTableInfo : ITableInfo
    {
        string TableAlias { get; }

        Expression ResolveReference(SqlTable sqlTable, IMappingResolver mappingResolver,
            IMappingResolutionContext mappingResolutionContext, UniqueIdentifierGenerator uniqueIdentifierGenerator);
    }
}