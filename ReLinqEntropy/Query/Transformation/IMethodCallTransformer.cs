using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ReLinqEntropy.Query.Transformation
{
    public interface IMethodCallTransformer
    {
        Expression Transform(MethodCallExpression methodCallExpression);
    }
}
