using Microsoft.Data.Sqlite;
using System.Data;

namespace SLib.SQL
{
    public class SQL
    {
        private readonly string _connectionString;
        private readonly string _dbFilePath;

        /// <summary>
        /// Init db, If .db not found, it is created.
        /// </summary>
        /// <param name="db">db file path</param>
        public SQL(string db)
        {
            _dbFilePath = Path.GetFullPath(db);

            string directory = Path.GetDirectoryName(_dbFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = db
            };

            _connectionString = connectionStringBuilder.ConnectionString;
        }

        public bool Exists()
        {
            return File.Exists(_dbFilePath);
        }

        /// <summary>
        /// Checks if a specific table exists in the database.
        /// </summary>
        public bool TableExists(string tableName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name=$name;";
                    command.Parameters.AddWithValue("$name", tableName);

                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Checks if a specific column exists within a given table.
        /// </summary>
        public bool ColumnExists(string tableName, string columnName)
        {
            // First, make sure the table even exists to avoid errors
            if (!TableExists(tableName)) return false;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // PRAGMA table_info returns meta-information about the table columns
                    command.CommandText = $"PRAGMA table_info({tableName});";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // The "name" column in the pragma results holds the column names
                            string currentColumnName = reader.GetString(reader.GetOrdinal("name"));

                            if (string.Equals(currentColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Executes a query that doesnt return any records (CREATE, INSERT, UPDATE, DELETE)
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    AddParameters(command, parameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar value (e.g., SELECT COUNT(*)).
        /// </summary>
        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    AddParameters(command, parameters);
                    return command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Executes a SELECT query and yields data row-by-row using a custom mapping function.
        /// </summary>
        public IEnumerable<T> ExecuteQuery<T>(string sql, Func<IDataRecord, T> mapFunction, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    AddParameters(command, parameters);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return mapFunction(reader);
                        }
                    }
                }
            }
        }

        public T ExecuteQueryFirst<T>(string sql, Func<IDataRecord, T> mapFunction, Dictionary<string, object> parameters = null)
        {
            T t = ExecuteQuery<T>(sql, mapFunction, parameters).First();
            return t;
        }

        // Helper method to safely attach parameters to prevent SQL Injection
        private void AddParameters(SqliteCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                // Handle null
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }
    }
}
