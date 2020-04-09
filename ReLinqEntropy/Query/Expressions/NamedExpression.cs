using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReLinqEntropy.Query.Expressions
{
    public class NamedExpression : Expression
    {
        public const string DefaultName = "value";
        private readonly string _name;
        private readonly Expression _expression;

        public static NamedExpression CreateFromMemberName(string memberName, Expression innerExpression)
            => new NamedExpression(memberName, innerExpression);

        public static Expression CreateNewExpressionWithNamedArguments(NewExpression newExpression, IEnumerable<Expression> processedArguments)
        {
            var newArguments = processedArguments.Select((e, i) => WrapIntoNamedExpression(GetMemberName(newExpression.Members, i), e)).ToArray();

            if (!newArguments.SequenceEqual(newExpression.Arguments))
            {
                if (newExpression.Members != null)
                {
                    return New(newExpression.Constructor, newArguments, newExpression.Members);
                }

                return New(newExpression.Constructor, newArguments);
            }

            return newExpression;
        }

        private static Expression WrapIntoNamedExpression(string memberName, Expression argumentExpression)
        {
            if (argumentExpression is NamedExpression expressionAsNamedExpression && expressionAsNamedExpression != null && expressionAsNamedExpression.Name == memberName)
            {
                return expressionAsNamedExpression;
            }

            return CreateFromMemberName(memberName, argumentExpression);
        }

        private static string GetMemberName(ReadOnlyCollection<MemberInfo> members, int index)
        {
            if (members == null || members.Count <= index)
            {
                return "M" + index;
            }

            return StripGetPrefix(members[index].Name);
        }

        // TODO: Move to utilities
        private static string StripGetPrefix(string memberName)
        {
            if (memberName.StartsWith("get_") && memberName.Length > 4)
            {
                memberName = memberName.Substring(4);
            }

            return memberName;
        }

        public NamedExpression(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _expression.Type;

        public string Name => _name;

        public Expression Expression => _expression;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newExpression = expressionVisitor.Visit(_expression);
            if (newExpression != _expression)
            {
                return new NamedExpression(_name, newExpression);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is INamedExpressionVisitor namedExpressionVisitor && namedExpressionVisitor != null)
            {
                return namedExpressionVisitor.VisitNamed(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString() => $"{_expression} AS {_name ?? DefaultName}";
    }
}
