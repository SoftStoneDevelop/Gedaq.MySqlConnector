using Benchmark.Helpers;
using DotNet.Testcontainers.Builders;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Testcontainers.MySql;

namespace Benchmark.Benchmarks
{
    public abstract class BenchmarkBase
    {
        public static MySqlDataSource _mySqlDataSource;

        private MySqlContainer _mysql;

        public async Task OneTimeSetUp()
        {
            _mysql =
                new MySqlBuilder("mysql:9.7.0")
                .WithUsername("root")
                .WithPassword("dhgvbh73j")
                .WithPortBinding(3306, true)
                .WithAutoRemove(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilExternalTcpPortIsAvailable(3306))
                .Build();

            await _mysql.StartAsync();
            await _mysql.WaitContainerStateRunningAsync(TimeSpan.FromMinutes(1));
            await _mysql.WaitResponseAsync(TimeSpan.FromMinutes(1));

            await using (var masterConnection = new MySqlConnection(_mysql.GetConnectionString()))
            {
                await masterConnection.OpenAsync();
                await using var createCmd = masterConnection.CreateCommand();
                createCmd.CommandText = $@"
CREATE DATABASE IF NOT EXISTS benchmark;
";
                createCmd.ExecuteNonQuery();
            }

            var builder = new MySqlConnectionStringBuilder(_mysql.GetConnectionString());
            builder.Database = "benchmark";
            builder.AllowLoadLocalInfile = true;

            _mySqlDataSource = new MySqlDataSource(builder.ConnectionString);

            await using var connection = await _mySqlDataSource.OpenConnectionAsync();
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
SET GLOBAL local_infile = 1;
";
                cmd.ExecuteNonQuery();
            }

            CreateIdentificationTable(connection);
            CreatePersonTable(connection);
            FillIndetification(connection);
            FillPerson(connection);
        }

        protected async Task OneTimeTearDown()
        {
            var dataSource = _mySqlDataSource;
            if (dataSource != null)
            {
                try
                {
                    await dataSource.DisposeAsync();
                }
                catch
                {
                    // ignore
                }
            }

            if (_mysql != null)
            {
                try
                {
                    await _mysql.DisposeAsync();
                }
                catch
                {
                    // ignore
                }
            }
        }

        private static void CreateIdentificationTable(MySqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE benchmark.identification
(
    id int NOT NULL,
    typename text NOT NULL,
    CONSTRAINT identification_pkey PRIMARY KEY (id)
)
";
            cmd.ExecuteNonQuery();
        }

        private static void CreatePersonTable(MySqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE benchmark.person
(
    id int NOT NULL PRIMARY KEY,
    firstname text,
    middlename text,
    lastname text,
    identification_id int,
    FOREIGN KEY (identification_id) REFERENCES benchmark.identification(id)
)
";
            cmd.ExecuteNonQuery();
        }

        private static void FillIndetification(MySqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            {
                cmd.CommandText = @"
INSERT INTO benchmark.identification
(
	id,
    typename
)
VALUES
(
    @id,
    @typename
);
";
                var id = cmd.CreateParameter();
                id.MySqlDbType = MySqlDbType.Int32;
                id.ParameterName = "id";
                cmd.Parameters.Add(id);

                var typename = cmd.CreateParameter();
                typename.MySqlDbType = MySqlDbType.Text;
                typename.ParameterName = "typename";
                cmd.Parameters.Add(typename);
                cmd.Prepare();

                id.Value = 1;
                typename.Value = "sailor's passport";
                cmd.ExecuteNonQuery();

                id.Value = 2;
                typename.Value = "officer's certificate";
                cmd.ExecuteNonQuery();

                id.Value = 3;
                typename.Value = "driver license";
                cmd.ExecuteNonQuery();

                id.Value = 4;
                typename.Value = "citizen's passport";
                cmd.ExecuteNonQuery();

                id.Value = 5;
                typename.Value = "party card";
                cmd.ExecuteNonQuery();
            }
        }

        private static void FillPerson(MySqlConnection connection)
        {
            var refId = 0;
            var setNull = false;

            DataTable table = new();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("firstname", typeof(string));
            table.Columns.Add("middlename", typeof(string));
            table.Columns.Add("lastname", typeof(string));
            table.Columns.Add("identification_id", typeof(int));

            var rows = new List<DataRow>();
            for (int i = 0; i < 50_000; i++)
            {
                var newRow = table.NewRow();
                newRow["id"] = i;
                newRow["firstname"] = $"John{i}";
                newRow["middlename"] = $"Сurly{i}";
                newRow["lastname"] = $"Doe{i}";

                if (++refId > 5)
                {
                    refId = 1;
                    setNull = true;
                }

                if (setNull)
                {
                    newRow["identification_id"] = DBNull.Value;
                    setNull = false;
                }
                else
                {
                    newRow["identification_id"] = refId;
                }

                rows.Add(newRow);
            }

            var bulkCopy = new MySqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "benchmark.person";
            var result = bulkCopy.WriteToServer(rows, 5);
            if (result.RowsInserted != 50_000)
            {
                throw new Exception($"RowsInserted:{result.RowsInserted}");
            }
        }
    }
}
