Constructors:

```C#

public ParametrAttribute(
  string methodName,
  Type parametrType,
  string parametrName = null,
  NpgsqlDbType dbType = NpgsqlDbType.Unknown,
  int size = -1,
  bool nullable = false,
  ParameterDirection direction = ParameterDirection.Input,
  int position = -1,
  string sourceColumn = "",
  bool sourceColumnNullMapping = false,
  DataRowVersion sourceVersion = DataRowVersion.Current,
  byte scale = 0,
  byte precision = 0
  )

```
Unique parametrs:<br>
`position`: Parameter position for unnamed parameters<br>

Rest parametrs and usage same as [Parametr](https://github.com/SoftStoneDevelop/Gedaq.DbConnection/blob/main/Documentation/Parametr.md).
