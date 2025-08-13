using Npgsql;

namespace AccountService.Shared.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddHangfireDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("hangfire-db");

        if (string.IsNullOrWhiteSpace(connString))
            throw new NullReferenceException("Hangfire DB connection string not found");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connString);
        var databaseName = connectionStringBuilder.Database;
        connectionStringBuilder.Database = "postgres";


        using var connection = new NpgsqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        using var checkCommand = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{databaseName}'", connection);
        var exists = (int?)checkCommand.ExecuteScalar() == 1;

        // ReSharper disable once InvertIf
        // не вижу проблем в данном if
        if (!exists)
        {
            using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName};", connection);
            command.ExecuteNonQuery();
        }
    }
}