using System;
using System.Linq;

namespace ReLinqEntropy.Query.Mapping
{
    public class SqlTableConcrete : SqlTable
    {
        private ITableInfo _tableInfo;

        public SqlTableConcrete(ITableInfo tableInfo, JoinSemantics joinSemantics) : base(tableInfo.ItemType,
            joinSemantics) => _tableInfo = tableInfo;

        public ITableInfo TableInfo
        {
            get => _tableInfo;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_tableInfo?.ItemType != value.ItemType)
                {
                    throw new InvalidOperationException(ExceptionMessageConstants.WrongJoinType);
                }

                _tableInfo = value;
            }
        }

        public override IResolvedTableInfo GetResolvedTableInfo() => TableInfo.GetResolvedTableInfo();

        public override void Accept(ISqlTableVisitor visitor) => visitor.VisitSqlTable(this);

        public override string ToString() => TableInfo + JoinedTables.Aggregate(string.Empty, (s, t) => s + " " + t);
    }
}