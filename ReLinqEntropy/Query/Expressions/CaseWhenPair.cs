using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ReLinqEntropy.Query.Expressions
{
    public class CaseWhenPair
    {
        private readonly Expression _when;
        private readonly Expression _then;

        public CaseWhenPair(Expression when, Expression then)
        {
            if (!IsBooleanType(when.Type))
            {
                throw new ArgumentException("When expression is not boolean", nameof(when));
            }

            _when = when;
            _then = then;
        }

        private bool IsBooleanType(Type type) =>
            type == typeof(bool) || type == typeof(bool?);

        public Expression When => _when;

        public Expression Then => _then;

        public CaseWhenPair VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newWhen = expressionVisitor.Visit(_when);
            var newThen = expressionVisitor.Visit(_then);

            return Update(newWhen, newThen);
        }

        public CaseWhenPair Update(Expression newWhen, Expression newThen)
        {
            if (newWhen != _when || newThen != _then)
            {
                return new CaseWhenPair(newWhen, newThen);
            }

            return this;
        }

        public override string ToString() => $"WHEN {_when} THEN {_then}";
    }
}
