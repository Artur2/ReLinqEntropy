using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlFunctionExpression : Expression
    {
        private readonly Type _type;
        private readonly string _sqlFunctionName;
        private readonly ReadOnlyCollection<Expression> _args;

        public SqlFunctionExpression(Type type, string sqlFunctionName, params Expression[] args)
        {
            _type = type;
            _args = Array.AsReadOnly(args);
            _sqlFunctionName = sqlFunctionName;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public string SqlFunctionName => _sqlFunctionName;

        public ReadOnlyCollection<Expression> Args => _args;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
        {
            var newArgs = expressionVisitor.VisitAndConvert(_args, "SqlFunctionExpression.VisitChildren");

            if (_args != newArgs)
            {
                return new SqlFunctionExpression(Type, _sqlFunctionName, newArgs.ToArray());
            }

            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlSpecificExpressionVisitor specificExpressionVisitor && specificExpressionVisitor != null)
            {
                return specificExpressionVisitor.VisitSqlFunction(this);
            }

            return base.Accept(visitor);
        }

        public override string ToString()
            => $"{_sqlFunctionName}({string.Join(", ", _args.Select(arg => arg.ToString()))})";
    }
}
