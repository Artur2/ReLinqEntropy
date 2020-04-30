using System;
using System.Linq.Expressions;
using System.Reflection;
using ReLinqEntropy.Internal;
using ReLinqEntropy.Query.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Mapping
{
    public class SqlEntityRefMemberExpression : Expression
    {
        private readonly Type _type;
        private readonly SqlEntityExpression _originatingEntity;
        private readonly MemberInfo _memberInfo;

        public SqlEntityRefMemberExpression(SqlEntityExpression originatingEntity, MemberInfo memberInfo)
        {
            _type = ReflectionUtility.GetMemberReturnType(memberInfo);
            _originatingEntity = originatingEntity;
            _memberInfo = memberInfo;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public SqlEntityExpression OriginatingEntity => _originatingEntity;

        public MemberInfo MemberInfo => _memberInfo;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor) => this;

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlEntityRefMemberExpressionVisitor specificRefMemberExpressionVisitor && specificRefMemberExpressionVisitor != null)
            {
                return specificRefMemberExpressionVisitor.VisitSqlEntityRefMember(this);
            }

            return base.Accept(visitor);
        }

        public override string ToString() => $"{_originatingEntity}.[{_memberInfo.Name}]";
    }
}