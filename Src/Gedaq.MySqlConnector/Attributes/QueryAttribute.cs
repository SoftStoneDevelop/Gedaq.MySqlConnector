using Gedaq.Common.Enums;
using Gedaq.MySqlConnector.Enums;
using System;

namespace Gedaq.MySqlConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryAttribute : Attribute
    {
        public QueryAttribute(
            string query,
            string methodName,
            Type queryMapType = null,
            MethodType methodType = MethodType.Sync,
            SourceType sourceType = SourceType.MySqlConnection,
            QueryType queryType = QueryType.Read,
            bool generate = true,
            AccessModifier accessModifier = AccessModifier.AsContainingClass,
            AsyncResult asyncResultType = AsyncResult.ValueTask,
            Type asPartInterface = null,
            ReturnType returnType = ReturnType.Enumerable
            )
        {
        }
    }
}