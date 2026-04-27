using System;

namespace Gedaq.MySqlConnector.Attributes
{
    /// <summary>
    /// An attribute that specifies a request that should be part of a batch request.
    /// </summary>
    /// <seealso href="https://github.com/SoftStoneDevelop/Gedaq.MySqlConnector/blob/main/Documentation/QueryBatch.md"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class BatchPartAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName">The name of the request that becomes part of the batch request.</param>
        /// <param name="position">The position of the request in the request batch.</param>
        public BatchPartAttribute(
            string methodName,
            int position)
        {
        }
    }
}