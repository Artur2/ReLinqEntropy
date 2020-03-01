namespace ReLinqEntropy.Query.Mapping
{
    public enum JoinSemantics
    {
        None,
        /// <summary>
        /// For optional join without result
        /// </summary>
        Left,
        /// <summary>
        /// For hard join with skip records without joined rows
        /// </summary>
        Inner
    }
}