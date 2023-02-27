Constructors:

```C#

public QueryAttribute(
  string query,
  string methodName,
  Type queryMapType = null,
  MethodType methodType = MethodType.Sync,
  SourceType sourceType = SourceType.Connection,
  QueryType queryType = QueryType.Read,
  bool generate = true
  )

```
Unique parametrs:<br>
`sourceType`: source type(connection/NpgsqlDataSource)<br>

Rest parametrs and usage same as [Query](https://github.com/SoftStoneDevelop/Gedaq.DbConnection/blob/main/Documentation/Query.md).
