using System;
using ReLinqEntropy.Query.Configuration;
using ReLinqEntropy.Query.Operator;
using Xunit;

namespace ReLinqEntropy.Tests.Query.Configuration
{
    public class ResultOperatorHandlerRegistryTests
    {
        [Fact]
        public void GetItem_With_Null_Passed_Value_Should_Throw_Exception()
        {
            var registry = new ResultOperatorHandlerRegistry();

            Assert.Throws<ArgumentNullException>(() => registry.GetItem(null));
        }

        [Fact]
        public void GetItem_With_Already_Registered_Item_Should_Return_It()
        {
            var registry = new ResultOperatorHandlerRegistry();

            registry.Register(typeof(ResultOperatorHandlerRegistryTests), new ResultHandlerStub());

            var item = registry.GetItem(typeof(ResultOperatorHandlerRegistryTests));

            Assert.NotNull(item);
        }

        public class ResultHandlerStub : IResultOperatorHandler
        {
            public Type SupportedResultOperatorType => throw new NotImplementedException();
        }
    }
}
