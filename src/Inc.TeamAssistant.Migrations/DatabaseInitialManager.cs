using Dapper;
using Npgsql;

namespace Inc.TeamAssistant.Migrations;

public sealed class DatabaseInitialManager
{
    private readonly string _connectionString;

    public DatabaseInitialManager(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public void CreateDatabase(string userPassword)
    {
        if (string.IsNullOrWhiteSpace(userPassword))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userPassword));

        var createScriptTemplate = ReadEmbeddedScript("create-template.sql");
        var createScript = String.Format(createScriptTemplate, userPassword);

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Execute(createScript);
    }

    private static string ReadEmbeddedScript(string scriptName)
    {
        if (string.IsNullOrWhiteSpace(scriptName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(scriptName));

        var assembly = typeof(DatabaseInitialManager).Assembly;
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.scripts.{scriptName}");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}