using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Linq;

namespace ReLinqEntropy.Query
{
    public class QueryExecutor : IQueryExecutor
    {
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
