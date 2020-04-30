using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Contexts
{
    public interface ISqlPreparationContext
    {
        Expression GetExpressionMapping(Expression expression);
    }
}
