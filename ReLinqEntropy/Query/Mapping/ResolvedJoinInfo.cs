using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping
{
    public class ResolvedJoinInfo : IJoinInfo
    {
        private readonly IResolvedTableInfo _foreignTableInfo;
        private readonly Expression _joinCondition;

        public ResolvedJoinInfo(IResolvedTableInfo foreignTableInfo, Expression joinCondition)
        {
            _foreignTableInfo = foreignTableInfo;
            _joinCondition = joinCondition;
        }

        public IResolvedTableInfo ForeignTableInfo => _foreignTableInfo;

        public Expression JoinCondition => _joinCondition;

        public Type ItemType => _foreignTableInfo.ItemType;

        public IJoinInfo Accept(IJoinInfoVisitor joinInfoVisitor) => throw new NotImplementedException();

        public ResolvedJoinInfo GetResolvedJoinInfo() => this;

        public override string ToString() => $"{ForeignTableInfo} ON {JoinCondition}";
    }
}