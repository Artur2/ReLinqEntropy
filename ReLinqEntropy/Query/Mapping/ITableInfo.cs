using System;

namespace ReLinqEntropy.Query.Mapping
{
    public interface ITableInfo
    {
        Type ItemType { get; }

        IResolvedTableInfo GetResolvedTableInfo();

        ITableInfo Accept(ITableInfoVisitor tableInfoVisitor);
    }
}