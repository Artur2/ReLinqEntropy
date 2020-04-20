using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlCaseExpression : Expression
    {
        private readonly Type _type;
        private readonly ReadOnlyCollection<CaseWhenPair> _cases;
        private readonly Expression _elseCase;

        public static SqlCaseExpression CreateIfThenElse(Type type, Expression test, Expression thenCase, Expression elseCase)
        { 
            return new SqlCaseExpression(type, new[] { new CaseWhenPair(test, thenCase) }, elseCase);
        }

        public static SqlCaseExpression CreateIfThenElseNull(Type type, Expression test, Expression trueCase, Expression falseCase)
        {
            return new SqlCaseExpression(type, new[] { new CaseWhenPair(test, trueCase), new CaseWhenPair(Not(test), falseCase) }, Constant(null, type));
        }

        public SqlCaseExpression(Type type, IEnumerable<CaseWhenPair> cases, Expression elseCase)
        {
            if (elseCase == null && type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                throw new ArgumentException("When no ELSE case is give, the expressions result type must be nullable.", nameof(type));
            }

            var casesArray = cases.ToArray();
            if (casesArray.Any(c => !type.IsAssignableFrom(c.Then.Type)))
            {
                throw new ArgumentException("The THEN expression's types must match the expression type.", nameof(cases));
            }
            if (elseCase != null && !type.IsAssignableFrom(elseCase.Type))
            {
                throw new ArgumentException("The ELSE expression's type must match the expression type.", nameof(elseCase));
            }

            _type = type;
            _cases = Array.AsReadOnly(casesArray);
            _elseCase = elseCase;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public ReadOnlyCollection<CaseWhenPair> Cases => _cases;

        public Expression ElseCase => _elseCase;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newCases = ExpressionVisitor.Visit(_cases, p => p.VisitChildren(expressionVisitor));
            var newEleseCase = _elseCase != null ? expressionVisitor.Visit(_elseCase) : null;

            return Update(newCases, newEleseCase);
        }

        public SqlCaseExpression Update(ReadOnlyCollection<CaseWhenPair> newCases, Expression newElseCase)
        {
            if (newCases != _cases || newElseCase != _elseCase)
            {
                return new SqlCaseExpression(Type, newCases, newElseCase);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlCase(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("CASE");
            foreach (var caseWhenPair in _cases)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(caseWhenPair);
            }

            if (_elseCase != null)
            {
                stringBuilder.Append(" ELSE ");
                stringBuilder.Append(_elseCase);
            }
            stringBuilder.Append(" END");

            return stringBuilder.ToString();
        }
    }
}
