using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReLinqEntropy.Query.Mapping.Statements
{
    public interface ISqlCommandBuilder
    {
        ParameterExpression InMemoryProjectionRowParameter { get; }

        CommandParameter CreateParameter(object value);
        CommandParameter GetOrCreateParameter(ConstantExpression constantExpression);

        void Append(string stringToAppend);
        void AppendSeparated<T>(string separator, IEnumerable<T> values, Action<ISqlCommandBuilder, T> appender);
        void AppendIdentifier(string identifier);
        void AppendStringLiteral(string value);
        void AppendFormat(string stringToAppend, params object[] parameters);
        CommandParameter AppendParameter(object value);

        void SetInMemoryProjectionBody(Expression body);
        Expression GetInMemoryProjectionBody();

        string GetCommandText();
        CommandParameter[] GetCommandParameters();
        SqlCommandData GetCommand();
    }
}
