using System;

namespace Gedaq.MySqlConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DynamicParametrAttribute : Attribute
    {
        public DynamicParametrAttribute(bool isPositionParametrs = false)
        {
        }
    }
}
