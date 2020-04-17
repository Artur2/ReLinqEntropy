using ReLinqEntropy.Query.Mapping;
using ReLinqEntropy.Query.Mapping.Visitors;
using System;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Expressions
{
    public class SqlTableReferenceExpression : Expression
    {
        private readonly SqlTable _sqlTable;

        public SqlTableReferenceExpression(SqlTable sqlTable)
        {
            _sqlTable = sqlTable;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _sqlTable.ItemType;

        public SqlTable SqlTable => _sqlTable;

        protected override Expression VisitChildren(ExpressionVisitor expressionVisitor)
            => this;

        protected override Expression Accept(ExpressionVisitor expressionVisitor)
        {
            if (expressionVisitor is ISqlTableReferenceExpressionVisitor specificVisitor && specificVisitor != null)
            {
                return specificVisitor.VisitSqlTableReference(this);
            }

            return base.Accept(expressionVisitor);
        }

        public override string ToString()
        {
            if (_sqlTable is SqlTableConcrete sqlTableBaseAsSqlTable)
            {
                var resolvedTableInfo = sqlTableBaseAsSqlTable.TableInfo as IResolvedTableInfo;
                if (resolvedTableInfo != null)
                {
                    return "TABLE-REF(" + resolvedTableInfo.TableAlias + ")";
                }

                return "TABLE-REF(" + sqlTableBaseAsSqlTable.TableInfo.GetType().Name + "(" + sqlTableBaseAsSqlTable.TableInfo.ItemType.Name + "))";
            }

            if (_sqlTable is SqlJoinedTable sqlTableBaseAsSqlJoinedTable)
            {
                if (sqlTableBaseAsSqlJoinedTable.JoinInfo is ResolvedJoinInfo)
                {
                    return "TABLE-REF(" + sqlTableBaseAsSqlJoinedTable.JoinInfo.GetResolvedJoinInfo().ForeignTableInfo.TableAlias + ")";
                }

                return "TABLE-REF(" + sqlTableBaseAsSqlJoinedTable.JoinInfo.GetType().Name + "(" + sqlTableBaseAsSqlJoinedTable.JoinInfo.ItemType.Name + "))";
            }

            return "TABLE-REF (" + _sqlTable.GetType().Name + " (" + _sqlTable.ItemType.Name + "))";
        }
    }
}
