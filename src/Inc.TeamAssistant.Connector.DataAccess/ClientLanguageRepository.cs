using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class ClientLanguageRepository : IClientLanguageRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public ClientLanguageRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<LanguageId> Get(Guid botId, long personId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                b.supported_languages AS supportedlanguages
            FROM connector.bots AS b
            WHERE b.id = @bot_id;

            SELECT
                p.language_id AS languageid,
                p.last_use AS lastuse
            FROM connector.client_languages AS p
            WHERE p.person_id = @person_id;
            """,
            new
            {
                bot_id = botId,
                person_id = personId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        await using var query = await connection.QueryMultipleAsync(command);
        
        var supportedLanguages = (await query.ReadSingleAsync<IReadOnlyCollection<string>>()).ToArray();
        var personLanguages = await query.ReadAsync<(string LanguageId, DateTimeOffset LastUse)>();

        if (supportedLanguages.Length == 1)
            return new LanguageId(supportedLanguages[0]);
        
        var languages = personLanguages
            .Where(l => supportedLanguages.Contains(l.LanguageId, StringComparer.InvariantCultureIgnoreCase))
            .ToArray();
        var result = languages.Any()
            ? new LanguageId(languages.MaxBy(l => l.LastUse).LanguageId)
            : LanguageSettings.DefaultLanguageId;
        
        return result;
    }

    public async Task Upsert(
        Guid botId,
        long personId,
        LanguageId languageId,
        DateTimeOffset now,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(languageId);

        var command = new CommandDefinition(
            """
            INSERT INTO connector.client_languages AS p (person_id, language_id, last_use)
            VALUES (@person_id, @language_id, @last_use)
            ON CONFLICT (person_id, language_id) DO UPDATE SET
                last_use = EXCLUDED.last_use;
            """,
            new
            {
                person_id = personId,
                language_id = languageId.Value,
                last_use = now
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}