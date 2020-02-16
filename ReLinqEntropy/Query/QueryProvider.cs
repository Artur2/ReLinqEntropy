using System.Linq;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace ReLinqEntropy.Query
{
    public class QueryProvider<T> : QueryableBase<T>
    {
        public QueryProvider(IQueryProvider provider) : base(provider)
        {
        }

        public QueryProvider(IQueryParser queryParser, IQueryExecutor executor) : base(
            new DefaultQueryProvider(typeof(QueryProvider<>), queryParser, executor))
        {
        }
    }
}
