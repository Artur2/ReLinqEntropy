using System;
using System.Collections.Generic;
using System.Text;

namespace ReLinqEntropy.Query.Operator
{
    public interface IResultOperatorHandler
    {
        Type SupportedResultOperatorType { get; }

        // TODO: Add other members
    }
}
