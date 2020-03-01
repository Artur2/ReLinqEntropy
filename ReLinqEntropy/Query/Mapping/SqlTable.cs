using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReLinqEntropy.Query.Mapping
{
    public abstract class SqlTable
    {
        private readonly Dictionary<MemberInfo, SqlJoinedTable> _joinedTables =
            new Dictionary<MemberInfo, SqlJoinedTable>();

        private readonly Type _itemType;
        private readonly JoinSemantics _joinSemantics;

        protected SqlTable(Type itemType, JoinSemantics joinSemantics)
        {
            _itemType = itemType;
            _joinSemantics = joinSemantics;
        }

        public abstract void Accept(ISqlTableBaseVisitor visitor);

        public Type ItemType => _itemType;

        public IEnumerable<SqlJoinedTable> JoinedTables => _joinedTables.Values;

        public JoinSemantics JoinSemantics => _joinSemantics;

        public SqlJoinedTable GetOrAddLeftJoin(IJoinInfo joinInfo, MemberInfo memberInfo)
        {
            if (joinInfo == null)
            {
                throw new ArgumentNullException();
            }
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            if (!_joinedTables.ContainsKey(memberInfo))
            {
                _joinedTables.Add(memberInfo, new SqlJoinedTable(joinInfo, JoinSemantics.Left));
            }

            return _joinedTables[memberInfo];
        }

        public SqlJoinedTable GetJoin(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            return _joinedTables[memberInfo];
        }
    }
}