namespace ReLinqEntropy.Query.Mapping
{
    public interface IJoinInfoVisitor
    {
        IJoinInfo VisitUnresolvedJoinInfo(UnresolvedJoinInfo joinInfo);
        IJoinInfo VisitUnresolvedCollectionJoinInfo(UnresolvedCollectionJoinInfo joinInfo);
        IJoinInfo VisitResolvedJoinInfo(ResolvedJoinInfo joinInfo);
    }
}