using ReLinqEntropy.Query.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;
using Remotion.Linq.Parsing;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping
{
    public class SubStatementReferenceResolver : RelinqExpressionVisitor, IResolvedSqlExpressionVisitor, INamedExpressionVisitor, ISqlGroupingSelectExpressionVisitor
    {
        private readonly ResolvedSubStatementTableInfo _tableInfo;
        private readonly SqlTable _sqlTable;
        private readonly IMappingResolutionContext _context;

        public static Expression ResolveSubStatementReferenceExpression(
            Expression referencedExpression,
            ResolvedSubStatementTableInfo containingSubStatementTableInfo,
            SqlTable containingSqlTable,
            IMappingResolutionContext context)
        {
            var visitor = new SubStatementReferenceResolver(containingSubStatementTableInfo, containingSqlTable, context);
            var result = visitor.Visit(referencedExpression);

            return result;
        }

        protected SubStatementReferenceResolver(ResolvedSubStatementTableInfo tableInfo, SqlTable sqlTable, IMappingResolutionContext context)
        {
            _tableInfo = tableInfo;
            _sqlTable = sqlTable;
            _context = context;
        }

        public Expression VisitSqlEntity(SqlEntityExpression expression)
        {
            var reference = expression.CreateReference(_tableInfo.TableAlias, expression.Type);
            _context.AddSqlEntityMapping(reference, _sqlTable);

            return reference;
        }

        public Expression VisitNamed(NamedExpression expression)
            => new SqlColumnDefinitionExpression(expression.Type, _tableInfo.TableAlias, expression.Name ?? NamedExpression.DefaultName, false);

        protected override Expression VisitNew(NewExpression expression)
        {
            var resolvedArguments = expression.Arguments.Select(expr => ResolveChildExpression(expr));
            return NamedExpression.CreateNewExpressionWithNamedArguments(expression, resolvedArguments);
        }

        public Expression VisitSqlGroupingSelect(SqlGroupingSelectExpression expression)
        {
            var referenceToKeyExpression = ResolveChildExpression(expression.KeyExpression);
            var referenceToElementExpression = ResolveChildExpression(expression.ElementExpression);
            var referenceToAggregationExpressions = expression.AggregatingExpressions.Select(expr => ResolveChildExpression(expr));

            var newGroupingExpression = SqlGroupingSelectExpression.CreateWithNames(referenceToKeyExpression, referenceToElementExpression);
            foreach (var aggregationExpression in referenceToAggregationExpressions)
            {
                newGroupingExpression.AddAggregationExpressionWithName(aggregationExpression);
            }

            _context.AddGroupReferenceMapping(newGroupingExpression, _sqlTable);

            return newGroupingExpression;
        }

        Expression IResolvedSqlExpressionVisitor.VisitSqlColumn(SqlColumnExpression expression)
        {
            throw new InvalidOperationException("SqlColumnExpression is not valid at this point. (Must be wrapped within a NamedExpression.)");
        }

        Expression IResolvedSqlExpressionVisitor.VisitSqlEntityConstant(SqlEntityConstantExpression expression)
        {
            throw new InvalidOperationException("SqlEntityConstantExpression is not valid at this point. (Must be wrapped within a NamedExpression.)");
        }

        private Expression ResolveChildExpression(Expression childExpression)
            => ResolveSubStatementReferenceExpression(childExpression, _tableInfo, _sqlTable, _context);
    }
}
