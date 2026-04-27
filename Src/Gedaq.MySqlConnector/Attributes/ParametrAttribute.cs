using MySqlConnector;
using System;
using System.Data;

namespace Gedaq.MySqlConnector.Attributes
{
    /// <summary>
    /// An attribute for specifying a parameter passed in a request.
    /// </summary>
    /// <seealso href="https://github.com/SoftStoneDevelop/Gedaq.MySqlConnector/blob/main/Documentation/Parametr.md"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ParametrAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parametrType">The type of parameter added to the generated query</param>
        /// <param name="parametrName">Parameter name if it is a named parameter, there are also positional parameters <paramref name="position"/></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="nullable"></param>
        /// <param name="direction"></param>
        /// <param name="sourceColumn"></param>
        /// <param name="sourceColumnNullMapping"></param>
        /// <param name="sourceVersion"></param>
        /// <param name="scale"></param>
        /// <param name="precision"></param>
        /// <param name="methodParametrName">The name of the parameter in the generated method. If not specified, an abstract name will be used, such as parameter1, parameter2, etc.</param>
        public ParametrAttribute(
            Type parametrType,
            string parametrName,
            MySqlDbType dbType = MySqlDbType.VarChar,
            int size = -1,
            bool nullable = false,
            ParameterDirection direction = ParameterDirection.Input,
            string sourceColumn = "",
            bool sourceColumnNullMapping = false,
            DataRowVersion sourceVersion = DataRowVersion.Current,
            byte scale = 0,
            byte precision = 0,
            string? methodParametrName = null)
        {
        }
    }
}