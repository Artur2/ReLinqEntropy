using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using ReLinqEntropy.Internal;
using ReLinqEntropy.Query.Expressions;
using ReLinqEntropy.Query.Mapping.Statements;
using Remotion.Linq.Parsing;

namespace ReLinqEntropy.Query.Mapping.Visitors
{
    public class SqlGeneratingExpressionVisitor :
        ThrowingExpressionVisitor,
        IResolvedSqlExpressionVisitor,
        ISqlSpecificExpressionVisitor,
        ISqlSubStatementVisitor,
        ISqlCustomTextGeneratorExpressionVisitor,
        INamedExpressionVisitor,
        IAggregationExpressionVisitor,
        ISqlColumnExpressionVisitor,
        ISqlCollectionExpressionVisitor
    {
        private readonly ISqlCommandBuilder _sqlCommandBuilder;
        private readonly ISqlGenerationStage _sqlGenerationStage;
        private readonly BinaryExpressionTextGenerator _binaryExpressionTextGenerator;

        public static void GenerateSql(Expression expression, ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
        {
            var visitor = new SqlGeneratingExpressionVisitor(commandBuilder, stage);
            visitor.Visit(expression);
        }

        public SqlGeneratingExpressionVisitor(ISqlCommandBuilder sqlCommandBuilder, ISqlGenerationStage sqlGenerationStage)
        {
            _sqlCommandBuilder = sqlCommandBuilder;
            _sqlGenerationStage = sqlGenerationStage;
            _binaryExpressionTextGenerator = new BinaryExpressionTextGenerator(sqlCommandBuilder, this);
        }

        public ISqlCommandBuilder SqlCommandBuilder => _sqlCommandBuilder;

        public ISqlGenerationStage SqlGenerationStage => _sqlGenerationStage;

        public virtual Expression VisitSqlEntity(SqlEntityExpression expression)
        {
            _sqlCommandBuilder.AppendSeparated(",", expression.Columns, (cb, column) => AppendColumnForEntity(expression, column));
            return expression;
        }

        public Expression VisitAggregation(AggregationExpression expression)
        {

            if (expression.AggregationModifier == AggregationModifier.Count)
            {
                _sqlCommandBuilder.Append("COUNT(*)");
                return expression;
            }

            if (expression.AggregationModifier == AggregationModifier.Average)
            {
                _sqlCommandBuilder.Append("AVG");
            }
            else if (expression.AggregationModifier == AggregationModifier.Max)
            {
                _sqlCommandBuilder.Append("MAX");
            }
            else if (expression.AggregationModifier == AggregationModifier.Min)
            {
                _sqlCommandBuilder.Append("MIN");
            }
            else if (expression.AggregationModifier == AggregationModifier.Sum)
            {
                _sqlCommandBuilder.Append("SUM");
            }
            else
            {
                var message = $"Cannot generate SQL for aggregation '{expression.AggregationModifier}'. Expression: '{expression}'";
                throw new NotSupportedException(message);
            }

            _sqlCommandBuilder.Append("(");

            Visit(expression.Expression);
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public Expression VisitNamed(NamedExpression expression)
        {
            Visit(expression.Expression);

            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            _sqlCommandBuilder.Append("(");
            _binaryExpressionTextGenerator.GenerateSqlForBinaryExpression(expression);
            _sqlCommandBuilder.Append(")");
            return expression;
        }

        protected override Expression VisitUnary(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    if (BooleanUtility.IsBooleanType(expression.Operand.Type))
                        _sqlCommandBuilder.Append("NOT ");
                    else
                        _sqlCommandBuilder.Append("~");
                    break;
                case ExpressionType.Negate:
                    _sqlCommandBuilder.Append("-");
                    break;
                case ExpressionType.UnaryPlus:
                    _sqlCommandBuilder.Append("+");
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    break;
                default:
                    var message = $"Cannot generate SQL for unary expression '{expression}'.";
                    throw new NotSupportedException(message);
            }

            Visit(expression.Operand);

            return expression;
        }

        public Expression VisitSqlCase(SqlCaseExpression expression)
        {
            _sqlCommandBuilder.Append("CASE");

            foreach (var caseWhenPair in expression.Cases)
            {
                _sqlCommandBuilder.Append(" WHEN ");
                Visit(caseWhenPair.When);
                _sqlCommandBuilder.Append(" THEN ");
                Visit(caseWhenPair.Then);
            }

            if (expression.ElseCase != null)
            {
                _sqlCommandBuilder.Append(" ELSE ");
                Visit(expression.ElseCase);
            }

            _sqlCommandBuilder.Append(" END");

            return expression;
        }

        public Expression VisitSqlCollection(SqlCollectionExpression expression)
        {
            _sqlCommandBuilder.Append("(");

            if (expression.Items.Count == 0)
            {
                _sqlCommandBuilder.Append("SELECT NULL WHERE 1 = 0");
            }

            _sqlCommandBuilder.AppendSeparated(", ", expression.Items, (cb, item) => Visit(item));
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public Expression VisitSqlColumnDefinition(SqlColumnDefinitionExpression expression)
        {
            AppendColumn(expression.ColumnName, expression.OwningTableAlias, null);

            return expression;
        }

        public Expression VisitSqlColumnReference(SqlColumnReferenceExpression expression)
        {
            var firstColumn = expression.ReferencedEntity.Columns.FirstOrDefault();
            var referencedEntityName = firstColumn != null && firstColumn.ColumnName == "*" ? null : expression.ReferencedEntity.Name;
            AppendColumn(expression.ColumnName, expression.OwningTableAlias, referencedEntityName);

            return expression;
        }

        public Expression VisitSqlConvert(SqlConvertExpression expression)
        {
            _sqlCommandBuilder.Append("CONVERT");
            _sqlCommandBuilder.Append("(");
            _sqlCommandBuilder.Append(expression.GetSqlTypeName());
            _sqlCommandBuilder.Append(", ");
            Visit(expression.Source);
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            string message = $"The method '{expression.Method.DeclaringType}.{expression.Method.Name}' " +
                $"is not supported by this code generator, and no custom transformer has been registered. Expression: '{expression}'";
            throw new NotSupportedException(message);
        }

        public virtual Expression VisitSqlExists(SqlExistsExpression expression)
        {
            _sqlCommandBuilder.Append("EXISTS");
            _sqlCommandBuilder.Append("(");
            Visit(expression.Expression);
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public Expression VisitSqlCustomTextGenerator(SqlCustomTextGeneratorExpression expression)
        {
            expression.Generate(_sqlCommandBuilder, this, _sqlGenerationStage);

            return expression;
        }

        public Expression VisitSqlFunction(SqlFunctionExpression expression)
        {
            _sqlCommandBuilder.Append(expression.SqlFunctionName);
            _sqlCommandBuilder.Append("(");
            _sqlCommandBuilder.AppendSeparated(", ", expression.Args, (cb, exp) => Visit(exp));
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public Expression VisitSqlLength(SqlLengthExpression expression)
        {
            // Since the SQL LEN function ignores trailing blanks, we add one character and subtract 1 from the result.
            // LEN (x + '#') - 1

            var concatMethod = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(object), typeof(object) });
            var extendedString = Expression.Add(
                expression.Expression.Type == typeof(char) ? Expression.Convert(expression.Expression, typeof(object)) : expression.Expression,
                new SqlLiteralExpression("#"),
                concatMethod);

            var newExpression = Expression.Subtract(new SqlFunctionExpression(typeof(int), "LEN", extendedString), new SqlLiteralExpression(1));

            return Visit(newExpression);
        }

        public Expression VisitSqlLike(SqlLikeExpression expression)
        {
            Visit(expression.Left);
            _sqlCommandBuilder.Append(" LIKE ");
            Visit(expression.Right);
            _sqlCommandBuilder.Append(" ESCAPE ");
            Visit(expression.EscapeExpression);

            return expression;
        }

        public Expression VisitSqlLiteral(SqlLiteralExpression expression)
        {
            if (expression.Type == typeof(string))
            {
                _sqlCommandBuilder.AppendStringLiteral((string)expression.Value);
            }
            else
            {
                _sqlCommandBuilder.Append(Convert.ToString(expression.Value, CultureInfo.InvariantCulture));
            }

            return expression;
        }

        public virtual Expression VisitSqlIn(SqlInExpression expression)
        {
            Visit(expression.LeftExpression);
            _sqlCommandBuilder.Append(" IN ");
            Visit(expression.RightExpression);

            return expression;
        }

        public virtual Expression VisitSqlIsNull(SqlIsNullExpression expression)
        {
            _sqlCommandBuilder.Append("(");
            Visit(expression.Expression);
            _sqlCommandBuilder.Append(" IS NULL");
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public virtual Expression VisitSqlIsNotNull(SqlIsNotNullExpression expression)
        {
            _sqlCommandBuilder.Append("(");
            Visit(expression.Expression);
            _sqlCommandBuilder.Append(" IS NOT NULL");
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public Expression VisitSqlRowNumber(SqlRowNumberExpression expression)
        {
            _sqlCommandBuilder.Append("ROW_NUMBER() OVER (ORDER BY ");
            _sqlCommandBuilder.AppendSeparated(", ", expression.Orderings, _sqlGenerationStage.GenerateTextForOrdering);
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        public Expression VisitSqlSubStatement(SqlSubStatementExpression expression)
        {
            _sqlCommandBuilder.Append("(");
            _sqlGenerationStage.GenerateTextForSqlStatement(_sqlCommandBuilder, expression.SqlStatement);
            _sqlCommandBuilder.Append(")");

            return expression;
        }

        Expression IResolvedSqlExpressionVisitor.VisitSqlColumn(SqlColumnExpression expression) => VisitExtension(expression);

        Expression IResolvedSqlExpressionVisitor.VisitSqlEntity(SqlEntityExpression expression)
        {
            _sqlCommandBuilder.AppendSeparated(",", expression.Columns, (cb, column) => AppendColumnForEntity(expression, column));
            return expression;
        }

        Expression IResolvedSqlExpressionVisitor.VisitSqlEntityConstant(SqlEntityConstantExpression expression) =>
            throw new NotSupportedException("Cannot use Entity as constant, only when comparing with other entity");

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            if (expression.Value == null)
            {
                _sqlCommandBuilder.Append("NULL");
            }
            else
            {
                var parameter = _sqlCommandBuilder.GetOrCreateParameter(expression);
                _sqlCommandBuilder.Append(parameter.Name);
            }

            return expression;
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            _sqlCommandBuilder.AppendSeparated(", ", expression.Arguments, (cb, expr) => Visit(expr));

            return expression;
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod) => throw new NotImplementedException();

        protected virtual void AppendColumnForEntity(SqlEntityExpression entity, SqlColumnExpression column) => Visit(column);

        protected virtual void AppendColumn(string columnName, string prefix, string referencedEntityName)
        {
            if (columnName == "*")
            {
                _sqlCommandBuilder.AppendIdentifier(prefix);
                _sqlCommandBuilder.Append(".*");
            }
            else
            {
                _sqlCommandBuilder.AppendIdentifier(prefix);
                _sqlCommandBuilder.Append(".");
                if (referencedEntityName != null)
                {
                    _sqlCommandBuilder.AppendIdentifier(referencedEntityName + "_" + (columnName ?? NamedExpression.DefaultName));
                }
                else
                {
                    _sqlCommandBuilder.AppendIdentifier(columnName ?? NamedExpression.DefaultName);
                }
            }
        }
    }
}
