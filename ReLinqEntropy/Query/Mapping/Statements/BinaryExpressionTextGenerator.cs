using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ReLinqEntropy.Internal;

namespace ReLinqEntropy.Query.Mapping.Statements
{
    public class BinaryExpressionTextGenerator
    {
        private readonly ISqlCommandBuilder _sqlCommandBuilder;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly Dictionary<ExpressionType, string> _simpleOperatorRegistry;

        public BinaryExpressionTextGenerator(ISqlCommandBuilder sqlCommandBuilder, ExpressionVisitor expressionVisitor)
        {
            _sqlCommandBuilder = sqlCommandBuilder;
            _expressionVisitor = expressionVisitor;

            _simpleOperatorRegistry = new Dictionary<ExpressionType, string>
                                {
                                    { ExpressionType.Add, "+" },
                                    { ExpressionType.AddChecked, "+" },
                                    { ExpressionType.And, "&" },
                                    { ExpressionType.AndAlso, "AND" },
                                    { ExpressionType.Divide, "/" },
                                    { ExpressionType.ExclusiveOr, "^" },
                                    { ExpressionType.GreaterThan, ">" },
                                    { ExpressionType.GreaterThanOrEqual, ">=" },
                                    { ExpressionType.LessThan, "<" },
                                    { ExpressionType.LessThanOrEqual, "<=" },
                                    { ExpressionType.Modulo, "%" },
                                    { ExpressionType.Multiply, "*" },
                                    { ExpressionType.MultiplyChecked, "*" },
                                    { ExpressionType.Or, "|" },
                                    { ExpressionType.OrElse, "OR" },
                                    { ExpressionType.Subtract, "-" },
                                    { ExpressionType.SubtractChecked, "-" },
                                    { ExpressionType.Equal, "=" },
                                    { ExpressionType.NotEqual, "<>" }
                                };
        }

        public void GenerateSqlForBinaryExpression(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Coalesce:
                    GenerateSqlForPrefixOperator("COALESCE", expression.Left, expression.Right);
                    break;
                case ExpressionType.Power:
                    GenerateSqlForPrefixOperator("POWER", expression.Left, expression.Right);
                    break;
                default:
                    GenerateSqlForInfixOperator(expression.Left, expression.Right, expression.NodeType, expression.Type);
                    break;
            }
        }

        private void GenerateSqlForPrefixOperator(string sqlOperatorString, Expression left, Expression right)
        {
            _sqlCommandBuilder.Append(sqlOperatorString);
            _sqlCommandBuilder.Append(" (");
            _expressionVisitor.Visit(left);
            _sqlCommandBuilder.Append(", ");
            _expressionVisitor.Visit(right);
            _sqlCommandBuilder.Append(")");
        }

        private void GenerateSqlForInfixOperator(Expression left, Expression right, ExpressionType nodeType, Type expressionType)
        {
            if (nodeType == ExpressionType.And && BooleanUtility.IsBooleanType(expressionType))
            {
                GenerateSqlForInfixOperator(left, right, ExpressionType.AndAlso, expressionType);
            }
            else if (nodeType == ExpressionType.Or && BooleanUtility.IsBooleanType(expressionType))
            {
                GenerateSqlForInfixOperator(left, right, ExpressionType.OrElse, expressionType);
            }
            else if (nodeType == ExpressionType.ExclusiveOr && BooleanUtility.IsBooleanType(expressionType))
            {
                // SQL has no logical XOR operator, so we simulate: a XOR b <=> (a AND NOT b) OR (NOT a AND b)
                var exclusiveOrSimulationExpression = Expression.OrElse(
                    Expression.AndAlso(left, Expression.Not(right)),
                    Expression.AndAlso(Expression.Not(left), right));
                _expressionVisitor.Visit(exclusiveOrSimulationExpression);
            }
            else
            {
                string operatorString = GetRegisteredOperatorString(nodeType);

                _expressionVisitor.Visit(left);
                _sqlCommandBuilder.Append(" ");
                _sqlCommandBuilder.Append(operatorString);
                _sqlCommandBuilder.Append(" ");
                _expressionVisitor.Visit(right);
            }
        }

        private string GetRegisteredOperatorString(ExpressionType nodeType)
        {
            if (!_simpleOperatorRegistry.TryGetValue(nodeType, out var operatorString))
            {
                throw new NotSupportedException("The binary operator '" + nodeType + "' is not supported.");
            }

            return operatorString;
        }
    }
}
