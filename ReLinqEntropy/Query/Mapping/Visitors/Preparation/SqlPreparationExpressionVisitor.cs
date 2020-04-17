using ReLinqEntropy.Query.Expressions;
using ReLinqEntropy.Query.Mapping.Contexts;
using ReLinqEntropy.Query.Transformation;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.Linq.Expressions;

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

        public Expression VisitPartialEvaluationException(PartialEvaluationExceptionExpression partialEvaluationExceptionExpression)
        {
            throw new NotImplementedException();
        }

        public Expression VisitSqlSubStatement(SqlSubStatementExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
