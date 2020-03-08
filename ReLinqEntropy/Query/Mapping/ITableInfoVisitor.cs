namespace ReLinqEntropy.Query.Mapping
{
    public interface ITableInfoVisitor
    {
        ITableInfo VisitUnresolvedTableInfo(UnresolvedTableInfo tableInfo);
        ITableInfo VisitUnresolvedGroupReferenceTableInfo(UnresolvedGroupReferenceTableInfo tableInfo);

        ITableInfo VisitSqlJoinedTable(SqlJoinedTable joinedTable);

        ITableInfo VisitSimpleTableInfo(ResolvedSimpleTableInfo tableInfo);

        ITableInfo VisitSubStatementTableInfo(ResolvedSubStatementTableInfo tableInfo);
    }
}