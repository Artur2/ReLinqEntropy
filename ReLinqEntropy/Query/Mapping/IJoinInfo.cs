using System;

namespace ReLinqEntropy.Query.Mapping
{
    public interface IJoinInfo
    {
        Type ItemType { get; }

        IJoinInfo Accept(IJoinInfoVisitor joinInfoVisitor);

        ResolvedJoinInfo GetResolvedJoinInfo();
    }
}