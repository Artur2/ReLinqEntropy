using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ReLinqEntropy.Query.Mapping
{
    public class UnresolvedCollectionJoinInfo : IJoinInfo
    {
        private readonly Expression _sourceExpression;
        private readonly MemberInfo _memberInfo;
        private readonly Type _itemType;

        public Expression SourceExpression => _sourceExpression;

        public MemberInfo MemberInfo => _memberInfo;

        public Type ItemType => _itemType;

        public IJoinInfo Accept(IJoinInfoVisitor joinInfoVisitor) => joinInfoVisitor.VisitUnresolvedCollectionJoinInfo(this);

        public ResolvedJoinInfo GetResolvedJoinInfo() =>
            throw new InvalidOperationException("This join has not yet been resolved; call the resolution step first.");

        public override string ToString()
            => $"{MemberInfo.DeclaringType.Name}.{MemberInfo.Name}";
    }
}
