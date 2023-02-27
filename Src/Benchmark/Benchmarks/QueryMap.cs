using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Configuration;
using NpgsqlBenchmark.Model;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace MySqlConnector.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
    [HideColumns("Error", "StdDev", "Median", "RatioSD")]
    public class QueryMap
    {
        [Params(50, 100, 200)]
        public int Size;

        private MySqlConnection _connection;

        [GlobalSetup]
        public void Setup()
        {
            var root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: false)
                .Build()
                ;

            _connection = new MySqlConnection(root.GetConnectionString("SqlConnection"));
            _connection.Open();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _connection?.Dispose();
        }

        [Gedaq.MySqlConnector.Attributes.Query(
            @"
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
WHERE p.id = @id
",
            "ReadInnerMap",
            typeof(Person)
            )]
        [Gedaq.MySqlConnector.Attributes.Parametr("ReadInnerMap", parametrType: typeof(int), parametrName: "id")]
        [Benchmark(Description = $"Gedaq.MySqlConnector")]
        public void MySqlConnector()
        {
            for (int i = 0; i < Size; i++)
            {
                var persons = ((MySqlConnection)_connection).ReadInnerMap(50000).ToList();
            }
        }

        [Gedaq.DbConnection.Attributes.Query(
            @"
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
WHERE p.id = @id
",
            "ReadInnerMap",
            typeof(Person)
            )]
        [Gedaq.DbConnection.Attributes.Parametr("ReadInnerMap", parametrType: typeof(int), parametrName:"id")]
        [Benchmark(Baseline = true, Description = "Gedaq.DbConnection")]
        public void DbConnection()
        {
            for (int i = 0; i < Size; i++)
            {
                var persons = ((DbConnection)_connection).ReadInnerMap(50000).ToList();
            }
        }
    }
}
