using Dapper;
using Npgsql;

namespace Inc.TeamAssistant.Migrations;

public sealed class DatabaseInitialManager
{
    private readonly string _connectionString;

    public DatabaseInitialManager(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _connectionString = connectionString;
    }

    public void CreateDatabase(string userPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userPassword);

        var createScriptTemplate = ReadEmbeddedScript("create-template.sql");
        var createScript = String.Format(createScriptTemplate, userPassword);

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Execute(createScript);
    }

    private static string ReadEmbeddedScript(string scriptName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scriptName);

        var assembly = typeof(DatabaseInitialManager).Assembly;
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.scripts.{scriptName}");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}