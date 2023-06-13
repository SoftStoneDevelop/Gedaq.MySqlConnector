Constructors:

```C#

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
  string methodParametrName = null
  )

```
Unique parametrs:<br>
`position`: Parameter position for unnamed parameters<br>

Rest parametrs and usage same as [Parametr](https://github.com/SoftStoneDevelop/Gedaq.DbConnection/blob/main/Documentation/Parametr.md).
