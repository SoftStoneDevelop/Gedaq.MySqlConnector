Constructors:

```C#

public BinaryImportAttribute(
        string query,
        string methodName,
        Type queryMapType,
        NpgsqlDbType[] dbTypes = null,
        MethodType methodType = MethodType.Sync,
        SourceType sourceType = SourceType.Connection
        )

```
Parametrs:<br>
`query`: sql query<br>
`methodName`: name of the generated method<br>
`queryMapType`: Type of result mapping collection<br>
`dbTypes`: postgresql databese types<br>
`methodType`: type of generated method(sync/async, flags enum)<br>
`sourceType`: type of connection source<br>

Model classes in example:
```C#

public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public Identification Identification { get; set; }
}

public class Identification
{
    public int Id { get; set; }
    public string TypeName { get; set; }
}

```

Usage from table:

```C#

[BinaryImport(@"
COPY person 
(
id,
firstname,
~StartInner::Identification:id~
    ~Reinterpret::id~
identification_id,
~EndInner::Identification~
middlename,
lastname
) 
FROM STDIN (FORMAT BINARY)
", 
            "BinaryImport",
            typeof(Person),
            new NpgsqlDbType[] 
            { 
                NpgsqlDbType.Integer,//id
                NpgsqlDbType.Text,//firstname
                NpgsqlDbType.Integer,//identification_id
                NpgsqlDbType.Text,//middlename
                NpgsqlDbType.Text//lastname
            },
            Gedaq.Common.Enums.MethodType.Sync | Gedaq.Common.Enums.MethodType.Async
            )]
public async Task SomeMethod(NpgsqlConnection connection, List<Person> list)
{
    connection.BinaryImport(list);
    await connection.BinaryImportAsync(list);
}
```
