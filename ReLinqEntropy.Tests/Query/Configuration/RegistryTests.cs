using System;
using System.Collections.Generic;
using ReLinqEntropy.Query.Configuration;
using Xunit;

namespace ReLinqEntropy.Tests.Query.Configuration
{
    public class RegistryTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("Test", null)]
        [InlineData(null, "Test")]
        public void Register_With_Null_Arguments_Should_Throw_Argument_Null_Exception(string key, string item)
        {
            var stub = new RegistryStub();

            Assert.Throws<ArgumentNullException>(() => stub.Register(key, item));
        }

        [Theory]
        [InlineData("Key1", "Test1")]
        [InlineData("Key2", "Test2")]
        public void Register_With_Arguments_Should_Add_To_Collection(string key, string item)
        {
            var stub = new RegistryStub();

            stub.Register(key, item);

            Assert.Contains(key, stub._items.Keys);
            Assert.Contains(item, stub._items.Values);
        }

        [Fact]
        public void Register_With_Multiple_Collection_Should_Throw_On_Wrong_Arguments()
        {
            var stub = new RegistryStub();

            Assert.Throws<ArgumentNullException>(() => stub.Register((IEnumerable<string>)null, null));
        }

        [Fact]
        public void Register_With_Passed_Collection_Should_Correspond_Add_It()
        {
            var stub = new RegistryStub();

            stub.Register(new[] { "key1", "key2", "key3" }, "test");

            Assert.Collection(stub._items,
                (keyValuePair) =>
                {
                    Assert.Equal("key1", keyValuePair.Key);
                    Assert.Equal("test", keyValuePair.Value);
                },
                (keyValuePair) =>
                {
                    Assert.Equal("key2", keyValuePair.Key);
                    Assert.Equal("test", keyValuePair.Value);
                },
                (keyValuePair) =>
                {
                    Assert.Equal("key3", keyValuePair.Key);
                    Assert.Equal("test", keyValuePair.Value);
                });
        }

        [Fact]
        public void GetExactItem_Should_Return_Exact_Item()
        {
            var stub = new RegistryStub();

            var item = "test";
            var exactItem = "test2";

            stub.Register("key1", item);
            stub.Register("key2", exactItem);

            var extracted = stub.GetItemExact("key2");

            Assert.Equal(exactItem, extracted);
        }

        public class RegistryStub : Registry<RegistryStub, string, string>
        {
            public override string GetItem(string key) => throw new NotImplementedException();

            internal protected override void RegisterForTypes(IEnumerable<Type> itemTypes)
            {
                throw new NotImplementedException();
            }
        }
    }
}
