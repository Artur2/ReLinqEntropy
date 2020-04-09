using ReLinqEntropy.Query.Expressions;
using Remotion.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ReLinqEntropy.Query.Mapping
{
    public interface IMappingResolver
    {
        /// <summary>
        /// Taking <paramref name="tableInfo"/> and <paramref name="uniqueIdentifierGenerator"/>
        /// to generate resolved table info
        /// </summary>
        IResolvedTableInfo ResolvedTableInfo(UnresolvedTableInfo tableInfo,
            UniqueIdentifierGenerator uniqueIdentifierGenerator);


        /// <summary>
        /// Takes an <see cref="UnresolvedJoinInfo"/> and an <see cref="UniqueIdentifierGenerator"/> to generate a 
        /// <see cref="ResolvedJoinInfo"/> that represents the join in the database.
        /// </summary>
        ResolvedJoinInfo ResolveJoinInfo(UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator);

        /// <summary>
        /// Analyzes the given <see cref="IResolvedTableInfo"/> and returns a <see cref="SqlEntityDefinitionExpression"/> , which represents the entity 
        /// described by the <paramref name="tableInfo"/> in the database. If the item type of the <paramref name="tableInfo"/> is not a 
        /// queryable entity, the resolver should throw an <see cref="UnmappedItemException"/>.
        /// </summary>
        SqlEntityDefinitionExpression ResolveSimpleTableInfo(IResolvedTableInfo tableInfo, UniqueIdentifierGenerator generator);

        /// <summary>
        /// Analyzes the given <see cref="MemberInfo"/> and returns an expression representing that member in the database. The resolved version will 
        /// usually be a <see cref="SqlColumnExpression"/> if the member represents a simple column, or a
        /// <see cref="SqlEntityRefMemberExpression"/> if the member references another entity.
        /// </summary>
        Expression ResolveMemberExpression(SqlEntityExpression originatingEntity, MemberInfo memberInfo);

        /// <summary>
        /// Analyzes a <see cref="MemberInfo"/> that is applied to a column and returns an expression representing that member in the database. The 
        /// resolved version will usually be a <see cref="SqlColumnExpression"/> if the member represents a simple column access, but it can also be any
        /// other expression if more complex calculations are needed.
        /// </summary>
        Expression ResolveMemberExpression(SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo);

        /// <summary>
        /// Analyses the given <see cref="ConstantExpression"/> and resolves it to a database-compatible expression if necessary. For example, if the 
        /// constant value is another entity, this method should return a <see cref="SqlEntityConstantExpression"/>.
        /// </summary>
        /// <param name="constantExpression">The <see cref="ConstantExpression"/> to be analyzed.</param>
        /// <returns>A resolved version of <paramref name="constantExpression"/>, usually a <see cref="SqlEntityConstantExpression"/>, or the
        /// <paramref name="constantExpression"/> itself.</returns>
        /// <exception cref="UnmappedItemException">The given <see cref="MemberInfo"/> cannot be resolved.</exception>
        /// <remarks>
        /// Note that compound expressions (<see cref="NewExpression"/> instances with named arguments) can be used to express compound entity identity. 
        /// Use <see cref="NamedExpression.CreateNewExpressionWithNamedArguments(System.Linq.Expressions.NewExpression)"/> to create a compound
        /// expression.
        /// </remarks>
        Expression ResolveConstantExpression(ConstantExpression constantExpression);

        /// <summary>
        /// Analyzes the given <see cref="Expression"/> and returns an expression that evaluates to <see langword="true" /> if it is of a desired 
        /// <see cref="Type"/>. This will usually be a comparison of a type identifier column with a constant value.
        /// </summary>
        Expression ResolveTypeCheck(Expression expression, Type desiredType);

        /// <summary>
        /// Analyzes the given <see cref="SqlEntityRefMemberExpression"/> and returns an expression that represents the optimized identity expression
        /// of the referenced entity if possible. The expression must be equivalent to the identity of the joined entity 
        /// (<see cref="SqlEntityExpression.GetIdentityExpression"/>). The purpose of this method is to avoid creating a join if the identity of the
        /// referenced entity can be inferred from the <see cref="SqlEntityRefMemberExpression.OriginatingEntity"/> (e.g., by analyzing a foreign key).
        /// </summary>
        Expression TryResolveOptimizedIdentity(SqlEntityRefMemberExpression entityRefMemberExpression);

        /// <summary>
        /// Analyzes the given <see cref="SqlEntityRefMemberExpression"/> and <see cref="MemberInfo"/> and returns an expression that represents an
        /// optimized version of the member acces on the referenced entity if possible. The expression must be equivalent to the result of
        /// <see cref="ResolveMemberExpression(Remotion.Linq.SqlBackend.SqlStatementModel.Resolved.SqlEntityExpression,System.Reflection.MemberInfo)"/>
        /// when executed on the resolved result of <paramref name="entityRefMemberExpression"/>. The purpose of this method is to avoid creating a join
        /// if a the member can be inferred from the <see cref="SqlEntityRefMemberExpression.OriginatingEntity"/> (e.g., by analyzing a foreign key).
        /// </summary>
        Expression TryResolveOptimizedMemberExpression(SqlEntityRefMemberExpression entityRefMemberExpression, MemberInfo memberInfo);
    }
}