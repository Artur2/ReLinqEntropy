using System;
using System.Linq.Expressions;
using ReLinqEntropy.Query.Mapping.Statements;
using ReLinqEntropy.Query.Mapping.Visitors;

namespace ReLinqEntropy.Query.Expressions
{
    public abstract class SqlCustomTextGeneratorExpression : Expression
    {
        private readonly Type _type;

        protected SqlCustomTextGeneratorExpression(Type type) => _type = type;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _type;

        public abstract void Generate(ISqlCommandBuilder sqlCommandBuilder, ExpressionVisitor textGeneratingExpresionVisitor, ISqlGenerationStage sqlGenerationStage);

        protected abstract override Expression VisitChildren(ExpressionVisitor expressionVisitor);

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlCustomTextGeneratorExpressionVisitor specificVisitor && specificVisitor != null)
            {
                return specificVisitor.VisitSqlCustomTextGenerator(this);
            }

            return base.Accept(visitor);
        }
    }
}
