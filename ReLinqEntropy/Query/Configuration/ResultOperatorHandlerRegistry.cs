using System;
using System.Collections.Generic;
using ReLinqEntropy.Query.Operator;

namespace ReLinqEntropy.Query.Configuration
{
    public class ResultOperatorHandlerRegistry : Registry<ResultOperatorHandlerRegistry, Type, IResultOperatorHandler>
    {
        public override IResultOperatorHandler GetItem(Type key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }


            IResultOperatorHandler handler;

            var currentType = key;
            do
            {
                handler = GetItemExact(currentType);
                currentType = currentType.BaseType;
            } while (handler == null && currentType != null);

            return handler;
        }

        internal protected override void RegisterForTypes(IEnumerable<Type> itemTypes)
        {
            if (itemTypes == null)
            {
                throw new ArgumentNullException(nameof(itemTypes));
            }

            foreach (var handlerType in itemTypes)
            {
                var handler = (IResultOperatorHandler)Activator.CreateInstance(handlerType);
                Register(handler.SupportedResultOperatorType, handler);
            }
        }
    }
}
