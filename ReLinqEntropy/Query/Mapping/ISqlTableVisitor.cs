namespace ReLinqEntropy.Query.Mapping
{
    public interface ISqlTableVisitor
    {
        void VisitSqlTable(SqlTableConcrete sqlTable);
        void VisitSqlJoinedTable(SqlJoinedTable joinedTable);
    }
}