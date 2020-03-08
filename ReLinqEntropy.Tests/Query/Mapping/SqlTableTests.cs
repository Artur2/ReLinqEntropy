using System;
using ReLinqEntropy.Query.Mapping;
using Xunit;

namespace ReLinqEntropy.Tests.Query.Mapping
{
    public class SqlTableTests
    {
        [Fact]
        public void GetOrAddLeftJoin_Should_Throw_Exception_On_Wrong_Arguments()
        {
            var stub = new SqlTableStub(typeof(SqlTableStub), JoinSemantics.None);

            Assert.Throws<ArgumentNullException>(() => stub.GetOrAddLeftJoin(null, null));
            Assert.Throws<ArgumentNullException>(() => stub.GetOrAddLeftJoin(new ResolvedJoinInfo(null, null), null));
            Assert.Throws<ArgumentNullException>(() => stub.GetOrAddLeftJoin(null, GetType()));
        }

        [Fact]
        public void GetOrAddLeftJoin_Should_Add_Item_If_It_Not_Exist()
        {
            var stub = new SqlTableStub(typeof(SqlTableStub), JoinSemantics.None);
            var joinInfo =
                new ResolvedJoinInfo(new ResolvedSimpleTableInfo(typeof(SqlTableStub), "Test", "Test"), null);
            var result = stub.GetOrAddLeftJoin(joinInfo, GetType());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetJoin_Should_Throw_On_Empty_Arguments()
        {
            var stub = new SqlTableStub(typeof(SqlTableStub), JoinSemantics.None);

            Assert.Throws<ArgumentNullException>(() => stub.GetJoin(null));
        }

        [Fact]
        public void GetJoin_Should_Return_Already_Added_Items()
        {
            var stub = new SqlTableStub(typeof(SqlTableStub), JoinSemantics.None);
            var joinInfo = new ResolvedSimpleTableInfo(typeof(SqlTableStub), "Test", "Test");
            var resolvedJoinInfo = new ResolvedJoinInfo(joinInfo, default);

            _ = stub.GetOrAddLeftJoin(resolvedJoinInfo, GetType());

            var result = stub.GetJoin(GetType());

            Assert.NotNull(result);
        }

        internal class SqlTableStub : SqlTable
        {
            public SqlTableStub(Type itemType, JoinSemantics joinSemantics) : base(itemType, joinSemantics)
            {
            }

            public override IResolvedTableInfo GetResolvedTableInfo()
            {
                throw new NotImplementedException();
            }

            public override void Accept(ISqlTableVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}