using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

internal sealed class RandomCoffeeReader : IRandomCoffeeReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public RandomCoffeeReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<RandomCoffeeEntry>> GetByDate(
        IReadOnlyCollection<RandomCoffeeState> activeStates,
        DateTimeOffset now,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(activeStates);
        
        var command = new CommandDefinition(@"
            SELECT
                e.id AS id,
                e.created AS created,
                e.bot_id AS botid,
                e.chat_id AS chatid,
                e.owner_id AS ownerid,
                e.next_round AS nextround,
                e.state AS state,
                e.poll_id AS pollid,
                e.participant_ids AS participantids,
                e.name AS name
            FROM random_coffee.entries AS e
            WHERE e.state = ANY(@active_states) AND e.next_round < @now;",
            new
            {
                active_states = activeStates.Select(i => (int)i).ToArray(),
                now
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<RandomCoffeeEntry>(command);
        return results.ToArray();
    }

    public async Task<IReadOnlyCollection<ChatDto>> GetChats(Guid botId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT DISTINCT
                e.chat_id AS chatid,
                e.name AS name
            FROM random_coffee.entries AS e
            WHERE e.bot_id = @bot_id;",
            new { bot_id = botId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var chats = await connection.QueryAsync<(long ChatId, string? Name)>(command);
        var results = chats
            .Select(c => new ChatDto(c.ChatId, c.Name ?? c.ChatId.ToString()))
            .ToArray();
        
        return results;
    }

    public async Task<IReadOnlyCollection<RandomCoffeeHistory>> GetHistory(
        Guid botId,
        long chatId,
        int depth,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                h.id AS id,
                h.created AS created,
                h.random_coffee_entry_id AS randomcoffeeentryid,
                h.pairs AS pairs,
                h.excluded_person_id AS excludedpersonid
            FROM random_coffee.history AS h
            JOIN random_coffee.entries AS e ON h.random_coffee_entry_id = e.id
            WHERE e.bot_id = @bot_id AND e.chat_id = @chat_id
            ORDER BY h.created DESC
            OFFSET 0
            LIMIT @depth;",
            new
            {
                bot_id = botId,
                chat_id = chatId,
                depth
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<RandomCoffeeHistory>(command);
        return results.ToArray();
    }
}