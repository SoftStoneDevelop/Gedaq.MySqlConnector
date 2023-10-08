Constructors:

```C#

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
  Type asPartInterface = null
  )

```
Unique parametrs:<br>
`sourceType`: source type(`MySqlConnection`/`MySqlDataSource`)<br>

Rest parametrs and usage same as [Query](https://github.com/SoftStoneDevelop/Gedaq.DbConnection/blob/main/Documentation/Query.md).
