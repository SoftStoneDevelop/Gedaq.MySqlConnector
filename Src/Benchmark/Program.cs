using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using MySqlConnector.Benchmarks;
using System;
using System.IO;

namespace MySqlConnector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //FillTestDatabase();
            BenchmarkRunner.Run<QueryMap>();
        }

        private static void FillTestDatabase()
        {
            var root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: false)
                .Build()
                ;

            using var connection = new MySqlConnection(root.GetConnectionString("SqlConnection"));
            connection.Open();
            CreateIdentificationTable(connection);
            CreatePersonTable(connection);
            FillIndetification(connection);
            FillPerson(connection);
        }

        private static void CreateIdentificationTable(MySqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE `gedaq`.`identification` (
  `id` INT NOT NULL,
  `typename` TEXT NOT NULL,
  PRIMARY KEY (`id`));
";
            cmd.ExecuteNonQuery();
        }

        private static void CreatePersonTable(MySqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE `gedaq`.`person` (
  `id` INT NOT NULL,
  `firstname` TEXT NULL,
  `middlename` TEXT NULL,
  `lastname` TEXT NULL,
  `identification_id` INT NULL,
  PRIMARY KEY (`id`),
  INDEX `identification_fk_idx` (`identification_id` ASC) VISIBLE,
  CONSTRAINT `identification_fk`
    FOREIGN KEY (`identification_id`)
    REFERENCES `gedaq`.`identification` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
";
            cmd.ExecuteNonQuery();
        }

        private static void FillIndetification(MySqlConnection connection)
        {
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"
INSERT INTO `gedaq`.`identification`
(`id`, `typename`)
VALUES
(@id, @typename);
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

        private static void FillPerson(MySqlConnection connection)
        {
            var transaction = connection.BeginTransaction();
            try
            {
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = @"
INSERT INTO `gedaq`.`person`
(`id`,
`firstname`,
`middlename`,
`lastname`,
`identification_id`)
	VALUES (
    @id,
    @firstname,
    @middlename,
    @lastname,
    @identification_id
);
";
                cmd.Parameters.Clear();
                var id = cmd.CreateParameter();
                id.MySqlDbType = MySqlDbType.Int32;
                id.ParameterName = "id";
                cmd.Parameters.Add(id);

                var firstname = cmd.CreateParameter();
                firstname.MySqlDbType = MySqlDbType.Text;
                firstname.ParameterName = "firstname";
                cmd.Parameters.Add(firstname);

                var middlename = cmd.CreateParameter();
                middlename.MySqlDbType = MySqlDbType.Text;
                middlename.ParameterName = "middlename";
                cmd.Parameters.Add(middlename);

                var lastname = cmd.CreateParameter();
                lastname.MySqlDbType = MySqlDbType.Text;
                lastname.ParameterName = "lastname";
                cmd.Parameters.Add(lastname);

                var identificationId = cmd.CreateParameter();
                identificationId.MySqlDbType = MySqlDbType.Int32;
                identificationId.ParameterName = "identification_id";
                identificationId.IsNullable = true;
                cmd.Parameters.Add(identificationId);
                cmd.Prepare();
                var refId = 0;
                var setNull = false;
                var millions = 0;
                var millionsCounter = 0;

                for (int i = 0; i < 1_000_000; i++)
                {
                    id.Value = i;
                    firstname.Value = $"John{i}";
                    middlename.Value = $"Сurly{i}";
                    lastname.Value = $"Doe{i}";

                    if (++refId > 5)
                    {
                        refId = 1;
                        setNull = true;
                    }

                    if (setNull)
                    {
                        identificationId.Value = DBNull.Value;
                        setNull = false;
                    }
                    else
                    {
                        identificationId.Value = refId;
                    }

                    if (++millionsCounter == 100_000)
                    {
                        millionsCounter = 0;
                        Console.WriteLine($"{++millions} 100_000 complete");
                    }

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }
    }
}