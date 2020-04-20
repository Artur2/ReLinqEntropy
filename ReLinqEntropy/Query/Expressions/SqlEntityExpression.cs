using ReLinqEntropy.Query.Mapping;
using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public abstract class SqlEntityExpression : Expression
    {
        private readonly Type _entityType;
        private readonly string _tableAlias;
        private readonly string _name;
        private readonly Func<SqlEntityExpression, Expression> _identityExpressionGenerator;

        protected SqlEntityExpression(
            Type entityType,
            string tableAlias,
            string name,
            Func<SqlEntityExpression, Expression> identityExpressionGenerator)
        {
            _entityType = entityType;
            _tableAlias = tableAlias;
            _name = name;
            _identityExpressionGenerator = identityExpressionGenerator;
        }

        public abstract ReadOnlyCollection<SqlColumnExpression> Columns { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _entityType;

        public string TableAlias => _tableAlias;

        public string Name => _name;

        public Func<SqlEntityExpression, Expression> IdentityExpressionGenerator => _identityExpressionGenerator;

        public abstract SqlColumnExpression GetColumn(Type type, string columnName, bool isPrimaryKeyColumn);

        public abstract SqlEntityExpression CreateReference(string newTableAlias, Type newType);

        public abstract SqlEntityExpression Update(Type itemType, string tableAlias, string entityName);

        public Expression GetIdentityExpression()
        {
            return _identityExpressionGenerator(this);
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is IResolvedSqlExpressionVisitor resolvedSqlExpressionVisitor)
            {
                return resolvedSqlExpressionVisitor.VisitSqlEntity(this);
            }

            return base.Accept(visitor);
        }
    }
}