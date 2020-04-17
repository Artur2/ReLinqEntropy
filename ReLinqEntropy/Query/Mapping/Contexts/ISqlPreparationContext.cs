using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ReLinqEntropy.Query.Mapping.Contexts
{
    public interface ISqlPreparationContext
    {
        Expression GetExpressionMapping(Expression expression);
    }
}
