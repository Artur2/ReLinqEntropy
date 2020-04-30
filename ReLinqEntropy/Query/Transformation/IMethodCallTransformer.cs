using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Transformation
{
    public interface IMethodCallTransformer
    {
        Expression Transform(MethodCallExpression methodCallExpression);
    }
}
