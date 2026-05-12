using Benchmark.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Gedaq.Common.Enums;
using MySqlConnector;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Benchmark.Benchmarks
{
    public class PersonFlat
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [Gedaq.Common.Attributes.IgnoreProperty()]
        public Identification Identification { get; set; }
    }

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net10_0)]
    public partial class CompareDapper : BenchmarkBase
    {
        private MySqlConnection _connection;

        [Params(1, 5, 10)]
        public int Iterations;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            await OneTimeSetUp();
        }

        [GlobalCleanup]
        public async Task GlobalCleanup()
        {
            await OneTimeTearDown();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _connection = _mySqlDataSource.OpenConnection();
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            try
            {
                _connection?.Dispose();
            }
            catch
            {
                // ignore
            }
            finally
            {
                _connection = null;
            }
        }

        [Benchmark(Baseline = true, Description = "Dapper")]
        public void Dapper()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = _connection.Query<Person, Identification, Person>(@"
SELECT 
    p.id,
    p.firstname,
    p.middlename,
    p.lastname,
    p.identification_id,
    i.typename
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
",
(person, ident) =>
{
    person.Identification = ident;
    return person;
},
splitOn: "identification_id").AsList();
            }
        }

        [Benchmark(Description = "Dapper Async")]
        public async Task DapperAsync()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = (await _connection.QueryAsync<Person, Identification, Person>(@"
SELECT 
    p.id,
    p.firstname,
    p.middlename,
    p.lastname,
    p.identification_id,
    i.typename
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
",
(person, ident) =>
{
    person.Identification = ident;
    return person;
},
splitOn: "identification_id")).AsList();
            }
        }

        [DapperAot]
        public static IEnumerable<Person> DapperAOTGetAllPerson(DbConnection connection) => connection.Query<Person, Identification, Person>(
        @"SELECT 
    p.id,
    p.firstname,
    p.middlename,
    p.lastname,
    p.identification_id,
    i.typename
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
",
(person, ident) =>
{
    person.Identification = ident;
    return person;
},
splitOn: "identification_id");

        [Benchmark(Description = "DapperAOT")]
        public void DapperAOT()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = DapperAOTGetAllPerson(_connection).AsList();
            }
        }

        [DapperAot]
        public static Task<IEnumerable<Person>> DapperAOTGetAllPersonAsync(DbConnection connection) => connection.QueryAsync<Person, Identification, Person>(
        @"SELECT 
    p.id,
    p.firstname,
    p.middlename,
    p.lastname,
    p.identification_id,
    i.typename
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
",
(person, ident) =>
{
    person.Identification = ident;
    return person;
},
splitOn: "identification_id");

        [Benchmark(Description = "DapperAOT Async")]
        public async Task DapperAOTAsync()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = (await DapperAOTGetAllPersonAsync(_connection)).AsList();
            }
        }

        [Gedaq.MySqlConnector.Attributes.Query(
            query: @"
SELECT 
    p.id,
    p.firstname,
~StartInner::Identification:id~
    i.id,
    i.typename,
~EndInner::Identification~
    p.middlename,
    p.lastname
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
",
            methodName: "GetAllPerson",
            queryMapTypes: [typeof(Person)],
            methodType: MethodType.Sync | MethodType.Async,
            asyncResultType: AsyncResult.ValueTask)]
        [Benchmark(Description = $"Gedaq Static Sync")]
        public void GedaqStatic()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = GetAllPerson(_connection);
            }
        }

        [Benchmark(Description = $"Gedaq Static Async")]
        public async Task GedaqStaticAsync()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = await GetAllPersonAsync(_connection);
            }
        }

        [Gedaq.MySqlConnector.Attributes.Query(
            query: null,
            methodName: "GetAllPersonDyn",
            queryMapTypes: [typeof(PersonFlat), typeof(Identification)],
            overrideAliasPrefixs: ["person_", "identity_"],
            methodType: MethodType.Sync | MethodType.Async,
            asyncResultType: AsyncResult.ValueTask)]
        [Benchmark(Description = $"Gedaq Dynamic Sync")]
        public void GedaqDynamic()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = new List<PersonFlat>();

                GetAllPersonDyn(_connection, @"
        SELECT 
            p.id as person_id,
            p.firstname as person_firstname,
            p.middlename as person_middlename,
            p.lastname as person_lastname,
            i.id as identity_id,
            i.typename as identity_typename
        FROM person p
        LEFT JOIN identification i ON i.id = p.identification_id
        ",
    (person, indetity) => { person.Identification = indetity; persons.Add(person); });
            }
        }

        [Benchmark(Description = $"Gedaq Dynamic Async")]
        public async Task GedaqDynamicAsync()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var persons = new List<PersonFlat>();

                await GetAllPersonDynAsync(_connection, @"
        SELECT 
            p.id as person_id,
            p.firstname as person_firstname,
            p.middlename as person_middlename,
            p.lastname as person_lastname,
            i.id as identity_id,
            i.typename as identity_typename
        FROM person p
        LEFT JOIN identification i ON i.id = p.identification_id
        ",
    (person, indetity) => { person.Identification = indetity; persons.Add(person); });
            }
        }
    }
}
