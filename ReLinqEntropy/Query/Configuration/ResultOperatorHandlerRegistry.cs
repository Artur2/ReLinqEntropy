using System;
using System.Collections.Generic;
using System.Text;
using ReLinqEntropy.Query.Operator;

namespace ReLinqEntropy.Query.Configuration
{
    public class ResultOperatorHandlerRegistry : Registry<ResultOperatorHandlerRegistry, Type, IResultOperatorHandler>
    {
        public override IResultOperatorHandler GetItem(Type key)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterForTypes(IEnumerable<Type> itemTypes)
        {
            throw new NotImplementedException();
        }
    }
}
