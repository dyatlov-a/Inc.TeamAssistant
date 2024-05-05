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
                b.owner_id AS ownerid
            FROM connector.bots AS b;",
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
                b.owner_id AS ownerid
            FROM connector.bots AS b
            WHERE b.id = @id;

            SELECT f.id AS id
            FROM connector.features AS f
            JOIN connector.activated_features AS af ON af.feature_id = f.id
            WHERE af.bot_id = @id;",
            new { id },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var query = await connection.QueryMultipleAsync(command);

        var bot = await query.ReadSingleOrDefaultAsync<Bot>();
        var features = await query.ReadAsync<Guid>();
        
        if (bot is not null)
            foreach (var feature in features)
                bot.AddFeature(feature);
        
        return bot;
    }
}