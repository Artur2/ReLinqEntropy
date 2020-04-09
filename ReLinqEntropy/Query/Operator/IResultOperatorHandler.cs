using System;

namespace ReLinqEntropy.Query.Operator
{
    public interface IResultOperatorHandler
    {
        Type SupportedResultOperatorType { get; }

        // TODO: Add other members
    }
}
