using System;

namespace Gedaq.MySqlConnector.Enums
{
    [Flags]
    public enum SourceType
    {
        MySqlConnection = 1,
        MySqlDataSource = 2
    }
}