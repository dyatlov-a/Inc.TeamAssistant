using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Domain;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Constructor.DataAccess;

internal sealed class BotRepository : IBotRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public BotRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<Bot>> GetBotsByOwner(long ownerId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                b.id AS id,
                b.name AS name,
                b.token AS token,
                b.owner_id AS ownerid,
                b.calendar_id AS calendarid,
                b.properties AS properties,
                b.supported_languages AS supportedlanguages
            FROM connector.bots AS b
            WHERE b.owner_id = @owner_id;",
            new
            {
                owner_id = ownerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<Bot>(command);
        return results.ToArray();
    }

    public async Task<Bot?> FindById(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                b.id AS id,
                b.name AS name,
                b.token AS token,
                b.owner_id AS ownerid,
                b.calendar_id AS calendarid,
                b.properties AS properties,
                b.supported_languages AS supportedlanguages
            FROM connector.bots AS b
            WHERE b.id = @id;

            SELECT f.id AS id
            FROM connector.features AS f
            JOIN connector.activated_features AS af ON af.feature_id = f.id
            WHERE af.bot_id = @id;",
            new
            {
                id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await using var query = await connection.QueryMultipleAsync(command);

        var bot = await query.ReadSingleOrDefaultAsync<Bot>();
        var features = await query.ReadAsync<Guid>();
        
        return bot?.ChangeFeatures(features.ToArray());
    }

    public async Task Upsert(Bot bot, CancellationToken token)
    {
        var upsertBotCommand = new CommandDefinition(@"
            INSERT INTO connector.bots (id, name, token, owner_id, calendar_id, properties, supported_languages)
            VALUES (@id, @name, @token, @owner_id, @calendar_id, @properties::jsonb, @supported_languages::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                token = EXCLUDED.token,
                owner_id = EXCLUDED.owner_id,
                calendar_id = EXCLUDED.calendar_id,
                properties = EXCLUDED.properties,
                supported_languages = EXCLUDED.supported_languages;",
            new
            {
                id = bot.Id,
                name = bot.Name,
                token = bot.Token,
                owner_id = bot.OwnerId,
                calendar_id = bot.CalendarId,
                properties = JsonSerializer.Serialize(bot.Properties),
                supported_languages = JsonSerializer.Serialize(bot.SupportedLanguages)
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        var deleteFeaturesCommand = new CommandDefinition(@"
            DELETE FROM connector.activated_features AS af
            WHERE af.bot_id = @bot_id;",
            new
            {
                bot_id = bot.Id
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        var insertFeaturesCommand = new CommandDefinition(@"
            INSERT INTO connector.activated_features (bot_id, feature_id)
            SELECT @bot_id, i.feature_id
            FROM UNNEST(@feature_ids) AS i(feature_id);",
            new
            {
                bot_id = bot.Id,
                feature_ids = bot.FeatureIds.ToArray()
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertBotCommand);
        await connection.ExecuteAsync(deleteFeaturesCommand);
        await connection.ExecuteAsync(insertFeaturesCommand);

        await transaction.CommitAsync(token);
    }

    public async Task Remove(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            DELETE FROM connector.bots AS b
            WHERE b.id = @id;",
            new
            {
                id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}