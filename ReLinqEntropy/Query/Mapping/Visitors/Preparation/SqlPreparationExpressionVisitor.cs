using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ReLinqEntropy.Query.Expressions;
using ReLinqEntropy.Query.Mapping.Contexts;
using ReLinqEntropy.Query.Mapping.Statements;
using ReLinqEntropy.Query.Transformation;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace ReLinqEntropy.Query.Mapping.Visitors.Preparation
{

    public class SqlPreparationExpressionVisitor : RelinqExpressionVisitor, ISqlSubStatementVisitor, IPartialEvaluationExceptionExpressionVisitor
    {
        private readonly ISqlPreparationContext _sqlPreparationContext;
        private readonly ISqlPreparationStage _sqlPreparationStage;
        private readonly IMethodCallTransformerProvider _methodCallTransformerProvider;

        public static Expression TranslateExpression(
            Expression expression,
            ISqlPreparationContext sqlPreparationContext,
            ISqlPreparationStage sqlPreparationStage,
            IMethodCallTransformerProvider methodCallTransformerProvider)
        {
            var visitor = new SqlPreparationExpressionVisitor(sqlPreparationContext, sqlPreparationStage, methodCallTransformerProvider);
            return visitor.Visit(expression);
        }

        protected SqlPreparationExpressionVisitor(
            ISqlPreparationContext sqlPreparationContext,
            ISqlPreparationStage sqlPreparationStage,
            IMethodCallTransformerProvider methodCallTransformerProvider)
        {
            _sqlPreparationContext = sqlPreparationContext;
            _sqlPreparationStage = sqlPreparationStage;
            _methodCallTransformerProvider = methodCallTransformerProvider;
        }

        public ISqlPreparationContext PreparationContext => _sqlPreparationContext;

        public ISqlPreparationStage PreparationStage => _sqlPreparationStage;

        public IMethodCallTransformerProvider MethodCallTransformerProvider => _methodCallTransformerProvider;

        public override Expression Visit(Expression expression)
        {
            if (expression != null)
            {
                var replacementExpression = _sqlPreparationContext.GetExpressionMapping(expression);
                if (replacementExpression != null)
                {
                    expression = replacementExpression;
                }
            }

            return base.Visit(expression);
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            if (expression.ReferencedQuerySource is GroupJoinClause)
            {
                var message = $"The results of a GroupJoin ('{expression.ReferencedQuerySource.ItemName}') can only be used as a query source, for example, in a from expression.";
                throw new NotSupportedException(message);
            }
            else
            {
                var message = $"The expression declaring identifier '{expression.ReferencedQuerySource.ItemName}' could not be found in the list of processed expressions. " +
                    $"Probably, the feature declaring '{expression.ReferencedQuerySource.ItemName}' isn't supported yet.";
                throw new KeyNotFoundException(message);
            }
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            var newExpression = _sqlPreparationStage.PrepareSqlStatement(expression.QueryModel, _sqlPreparationContext).CreateExpression();

            return Visit(newExpression);
        }

        public Expression VisitSqlSubStatement(SqlSubStatementExpression expression)
        {
            if (expression.SqlStatement.Orderings.Count > 0 && expression.SqlStatement.TopExpression == null)
            {
                var builder = new SqlStatementBuilder(expression.SqlStatement);
                builder.Orderings.Clear();
                return new SqlSubStatementExpression(builder.GetSqlStatement());
            }

            return expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            var newInnerExpression = Visit(expression.Expression);
            if (newInnerExpression is SqlCaseExpression sqlCaseExpression && sqlCaseExpression != null)
            {
                var originalCases = sqlCaseExpression.Cases;
                var originalElseCase = sqlCaseExpression.ElseCase;
                var newCases = originalCases.Select(x => new CaseWhenPair(x.When, Expression.MakeMemberAccess(x.Then, expression.Member)));
                var newElseCase = originalElseCase != null ? Expression.MakeMemberAccess(originalElseCase, expression.Member) : null;

                var caseExpressionType =
                    newElseCase == null && expression.Type.IsValueType && Nullable.GetUnderlyingType(expression.Type) == null
                    ? typeof(Nullable<>).MakeGenericType(expression.Type)
                    : expression.Type;

                var newSqlCaseExpression = new SqlCaseExpression(caseExpressionType, newCases, newElseCase);
                return Visit(newSqlCaseExpression);
            }

            if (newInnerExpression.NodeType == ExpressionType.Coalesce && newInnerExpression is BinaryExpression newInnerExpressionAsBinary && newInnerExpressionAsBinary != null)
            {
                var newConditionalExpression = Expression.Condition(
                    new SqlIsNotNullExpression(newInnerExpressionAsBinary.Left),
                    Expression.MakeMemberAccess(newInnerExpressionAsBinary.Left, expression.Member),
                    Expression.MakeMemberAccess(newInnerExpressionAsBinary.Right, expression.Member)
                    );

                return Visit(newConditionalExpression);
            }

            if (newInnerExpression is SqlSubStatementExpression newInnerExpressionAsSqlSubStatementExpression && newInnerExpressionAsSqlSubStatementExpression != null)
            {
                var sqlStatementBuilder = new SqlStatementBuilder(newInnerExpressionAsSqlSubStatementExpression.SqlStatement);
                var namedExpression = (NamedExpression)sqlStatementBuilder.SelectProjection;

                sqlStatementBuilder.SelectProjection = new NamedExpression(namedExpression.Name, Visit(Expression.MakeMemberAccess(namedExpression.Expression, expression.Member)));
                sqlStatementBuilder.RecalculateDataInfo(newInnerExpressionAsSqlSubStatementExpression.SqlStatement.SelectProjection);
                return new SqlSubStatementExpression(sqlStatementBuilder.GetSqlStatement());
            }

            if (expression.Member is PropertyInfo memberAsPropertyInfo && memberAsPropertyInfo != null)
            {
                var methodInfo = memberAsPropertyInfo.GetGetMethod();
                if (methodInfo != null)
                {
                    var methodCallExpression = Expression.Call(expression.Expression, methodInfo);
                    var transformer = _methodCallTransformerProvider.GetTransformer(methodCallExpression);
                    if (transformer != null)
                    {
                        var transformedExpression = transformer.Transform(methodCallExpression);
                        return Visit(transformedExpression);
                    }
                }
            }

            return base.VisitMember(expression);
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            if (IsNullConstant(expression.Left))
            {
                if (expression.NodeType == ExpressionType.Equal)
                {
                    return Visit(new SqlIsNullExpression(expression.Right));
                }
                if (expression.NodeType == ExpressionType.NotEqual)
                {
                    return Visit(new SqlIsNotNullExpression(expression.Right));
                }

                return base.VisitBinary(expression);
            }

            if (IsNullConstant(expression.Right))
            {
                if (expression.NodeType == ExpressionType.Equal)
                {
                    return Visit(new SqlIsNullExpression(expression.Left));
                }
                if (expression.NodeType == ExpressionType.NotEqual)
                {
                    return Visit(new SqlIsNotNullExpression(expression.Left));
                }

                return base.VisitBinary(expression);
            }

            return base.VisitBinary(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            var transofrmer = _methodCallTransformerProvider.GetTransformer(expression);
            if (transofrmer != null)
            {
                var transformedExpression = transofrmer.Transform(expression);
                return Visit(transformedExpression);
            }

            var namedInstance = expression.Object != null ? NamedExpression.CreateFromMemberName("Object", Visit(expression.Object)) : null;
            var namedArguments = expression.Arguments.Select((a, i) => (Expression)NamedExpression.CreateFromMemberName("Arg" + i, Visit(a)));

            return Expression.Call(namedInstance, expression.Method, namedArguments);
        }

        protected override Expression VisitConditional(ConditionalExpression expression) => SqlCaseExpression.CreateIfThenElse(expression.Type, Visit(expression.Test), Visit(expression.IfTrue), Visit(expression.IfFalse));

        protected override Expression VisitNew(NewExpression expression) => NamedExpression.CreateNewExpressionWithNamedArguments(expression, expression.Arguments.Select(Visit));

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            var collection = expression.Value as ICollection;
            if (collection != null)
            {
                return new SqlCollectionExpression(expression.Type, collection.Cast<object>().Select(Expression.Constant).Cast<Expression>());
            }

            return base.VisitConstant(expression);
        }

        public Expression VisitPartialEvaluationException(PartialEvaluationExceptionExpression partialEvaluationExceptionExpression) => Visit(partialEvaluationExceptionExpression.EvaluatedExpression);

        private bool IsNullConstant(Expression expression)
        {
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                if (constantExpression.Value == null)
                    return true;
            }
            return false;
        }
    }
}
