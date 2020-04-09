using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlGroupingSelectExpression : Expression
    {
        public static SqlGroupingSelectExpression CreateWithNames(Expression unnamedKeySelector, Expression unnamedElementSelector)
            => new SqlGroupingSelectExpression(
                new NamedExpression("unnamedKeySelector", unnamedKeySelector),
                new NamedExpression("unnamedElementSelector", unnamedElementSelector));

        public SqlGroupingSelectExpression(Expression keyExpression, Expression elementExpression) :
            this(keyExpression, elementExpression, new List<Expression>())
        {

        }

        private readonly Type _type;
        private readonly Expression _keyExpression;
        private readonly Expression _elementExpression;
        private readonly List<Expression> _aggregationExpressions;

        public SqlGroupingSelectExpression(Expression keyExpression, Expression elementExpression, IEnumerable<Expression> aggregationExpressions)
        {
            _type = typeof(IGrouping<,>).MakeGenericType(keyExpression.Type, elementExpression.Type);
            _keyExpression = keyExpression;
            _elementExpression = elementExpression;
            _aggregationExpressions = aggregationExpressions.ToList();
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public Expression KeyExpression => _keyExpression;

        public Expression ElementExpression => _elementExpression;

        public ReadOnlyCollection<Expression> AggregatingExpressions => _aggregationExpressions.AsReadOnly();

        public string AddAggregationExpressionWithName(Expression unnamedExpression)
        {
            var name = $"A{_aggregationExpressions.Count}";
            _aggregationExpressions.Add(new NamedExpression(name, unnamedExpression));

            return name;
        }

        public SqlGroupingSelectExpression Update(Expression newKeyExpression, Expression newElementExpression, IEnumerable<Expression> expressions)
            => new SqlGroupingSelectExpression(newKeyExpression, newElementExpression, expressions);

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newKeyExpression = expressionVisitor.Visit(KeyExpression);
            var newElementExpression = expressionVisitor.Visit(ElementExpression);

            var originalAggregatingExpressions = AggregatingExpressions;
            var newAggregationExpressions = expressionVisitor.VisitAndConvert(originalAggregatingExpressions, "VisitChildren");

            if (newKeyExpression != KeyExpression || newElementExpression != ElementExpression || newAggregationExpressions != originalAggregatingExpressions)
            {
                return new SqlGroupingSelectExpression(newKeyExpression, newElementExpression, newAggregationExpressions);
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlGroupingSelectExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlGroupingSelect(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString()
            => $"GROUPING (KEY: {KeyExpression}, ELEMENT: {ElementExpression}, AGGREGATIONS: ({string.Join(", ", AggregatingExpressions.Select(o => o.ToString()))}))";
    }
}
