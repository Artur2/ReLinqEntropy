using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Transformation
{
    public interface IMethodCallTransformerProvider
    {
        IMethodCallTransformer GetTransformer(MethodCallExpression methodCallExpression);
    }
}
