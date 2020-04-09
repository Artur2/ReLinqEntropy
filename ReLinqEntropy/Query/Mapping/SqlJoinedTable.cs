using System;
using System.Linq;

namespace ReLinqEntropy.Query.Mapping
{
    public class SqlJoinedTable : SqlTable, ITableInfo
    {
        private IJoinInfo _joinInfo;

        public SqlJoinedTable(IJoinInfo joinInfo, JoinSemantics joinSemantics) : base(joinInfo.ItemType, joinSemantics)
        {
            _joinInfo = joinInfo;
        }

        public IJoinInfo JoinInfo
        {
            get => _joinInfo;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_joinInfo?.ItemType != value.ItemType)
                {
                    throw new InvalidOperationException(ExceptionMessageConstants.WrongJoinType);
                }

                _joinInfo = value;
            }
        }

        public override void Accept(ISqlTableVisitor visitor)
        {
            visitor.VisitSqlJoinedTable(this);
        }

        public override IResolvedTableInfo GetResolvedTableInfo() => JoinInfo.GetResolvedJoinInfo().ForeignTableInfo;

        public ITableInfo Accept(ITableInfoVisitor tableInfoVisitor)
        {
            return tableInfoVisitor.VisitSqlJoinedTable(this);
        }

        public override string ToString() =>

        JoinSemantics.ToString().ToUpper() + " JOIN " + JoinInfo + JoinedTables.Aggregate("", (s, t) => s + " " + t);
    }
}