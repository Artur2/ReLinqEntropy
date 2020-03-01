using System;

namespace ReLinqEntropy.Query.Mapping
{
    public class SqlJoinedTable : SqlTable, ITableInfo
    {
        public SqlJoinedTable(IJoinInfo joinInfo, JoinSemantics joinSemantics) :base(joinInfo.ItemType, joinSemantics)
        {
            
        }

        public override void Accept(ISqlTableBaseVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public Type ItemType { get; }
        public IResolvedTableInfo GetResolvedTableInfo()
        {
            throw new NotImplementedException();
        }

        public ITableInfo Accept(ITableInfoVisitor tableInfoVisitor)
        {
            throw new NotImplementedException();
        }
    }
}