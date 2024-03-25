using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Languages;
using Npgsql;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class ClientLanguageRepository : IClientLanguageRepository
{
    private readonly string _connectionString;

    public ClientLanguageRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }
    
    public async Task<LanguageId> Get(long personId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.language_id AS languageid
            FROM connector.client_languages AS p
            WHERE p.person_id = @person_id;",
            new { person_id = personId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        var languageId = await connection.QuerySingleOrDefaultAsync<string?>(command);
        
        var result = languageId is not null ? LanguageId.Build(languageId) : null;
        return result ?? LanguageSettings.DefaultLanguageId;
    }

    public async Task Upsert(long personId, string languageId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(languageId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(languageId));

        var command = new CommandDefinition(@"
            INSERT INTO connector.client_languages AS p (person_id, language_id)
            VALUES (@person_id, @language_id)
            ON CONFLICT (person_id) DO UPDATE SET
                language_id = EXCLUDED.language_id;",
            new
            {
                person_id = personId,
                language_id = languageId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}