using System.Data.Common;
using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

internal sealed class RandomCoffeeRepository : IRandomCoffeeRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public RandomCoffeeRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<RandomCoffeeEntry?> Find(Guid id, CancellationToken token)
    {
        await using var connection = _connectionFactory.Create();

        return await Find(connection, id, token);
    }

    public async Task<RandomCoffeeEntry?> Find(string pollId, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pollId);
        
        var command = new CommandDefinition(
            """
            SELECT e.id AS id
            FROM random_coffee.entries AS e
            WHERE e.poll ->> 'Id' = @poll_id;
            """,
            new
            {
                poll_id = pollId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var randomCoffeeEntryId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);

        return randomCoffeeEntryId.HasValue
            ? await Find(connection, randomCoffeeEntryId.Value, token)
            : null;
    }

    public async Task<RandomCoffeeEntry?> Find(long chatId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT e.id AS id
            FROM random_coffee.entries AS e
            WHERE e.chat_id = @chat_id;
            """,
            new
            {
                chat_id = chatId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var randomCoffeeEntryId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);

        return randomCoffeeEntryId.HasValue
            ? await Find(connection, randomCoffeeEntryId.Value, token)
            : null;
    }

    public async Task Upsert(RandomCoffeeEntry randomCoffeeEntry, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(randomCoffeeEntry);

        var randomCoffeeEntryPoll = randomCoffeeEntry.Poll is not null
            ? JsonSerializer.Serialize(randomCoffeeEntry.Poll)
            : null;
        var historyId = new List<Guid>(randomCoffeeEntry.History.Count);
        var historyCreated = new List<DateTimeOffset>(randomCoffeeEntry.History.Count);
        var historyPairs = new List<string>(randomCoffeeEntry.History.Count);
        var historyExcludedPersonId = new List<long?>(randomCoffeeEntry.History.Count);

        foreach (var item in randomCoffeeEntry.History)
        {
            historyId.Add(item.Id);
            historyCreated.Add(item.Created);
            historyPairs.Add(JsonSerializer.Serialize(item.Pairs));
            historyExcludedPersonId.Add(item.ExcludedPersonId);
        }

        var participantIds = JsonSerializer.Serialize(randomCoffeeEntry.ParticipantIds);
        var upsertEntry = new CommandDefinition(
            """
            INSERT INTO random_coffee.entries (
                id,
                created,
                bot_id,
                chat_id,
                owner_id,
                next_round,
                state,
                poll,
                participant_ids,
                name)
            VALUES (
                @id,
                @created,
                @bot_id,
                @chat_id,
                @owner_id,
                @next_round,
                @state,
                @poll::jsonb,
                @participant_ids::jsonb,
                @name)
            ON CONFLICT (id) DO UPDATE SET
                created = excluded.created,
                bot_id = excluded.bot_id,
                chat_id = excluded.chat_id,
                owner_id = excluded.owner_id,
                next_round = excluded.next_round,
                state = excluded.state,
                poll = excluded.poll,
                participant_ids = excluded.participant_ids,
                name = excluded.name;
            """,
            new
            {
                id = randomCoffeeEntry.Id,
                created = randomCoffeeEntry.Created,
                bot_id = randomCoffeeEntry.BotId,
                chat_id = randomCoffeeEntry.ChatId,
                owner_id = randomCoffeeEntry.OwnerId,
                next_round = randomCoffeeEntry.NextRound,
                state = (int)randomCoffeeEntry.State,
                poll = randomCoffeeEntryPoll,
                participant_ids = participantIds,
                name = randomCoffeeEntry.Name
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        await connection.ExecuteAsync(upsertEntry);

        if (randomCoffeeEntry.History.Any())
        {
            var upsertHistory = new CommandDefinition(
                """
                INSERT INTO random_coffee.history (
                    id,
                    created,
                    random_coffee_entry_id,
                    pairs,
                    excluded_person_id)
                SELECT id, created, @random_coffee_entry_id, pairs::jsonb, excluded_person_id
                FROM UNNEST(@id, @created, @pairs, @excluded_person_id)
                    AS i(id, created, pairs, excluded_person_id)
                ON CONFLICT (id) DO UPDATE SET
                    created = excluded.created,
                    random_coffee_entry_id = excluded.random_coffee_entry_id,
                    pairs = excluded.pairs,
                    excluded_person_id = excluded.excluded_person_id;
                """,
                new
                {
                    id = historyId,
                    created = historyCreated,
                    random_coffee_entry_id = randomCoffeeEntry.Id,
                    pairs = historyPairs,
                    excluded_person_id = historyExcludedPersonId
                },
                flags: CommandFlags.None,
                cancellationToken: token);
            
            await connection.ExecuteAsync(upsertHistory);
        }

        await transaction.CommitAsync(token);
    }
    
    private async Task<RandomCoffeeEntry?> Find(DbConnection connection, Guid id, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(connection);
        
        const int historyDepth = 5;
        var command = new CommandDefinition(
            """
            SELECT
                e.id AS id,
                e.created AS created,
                e.bot_id AS botid,
                e.chat_id AS chatid,
                e.owner_id AS ownerid,
                e.next_round AS nextround,
                e.state AS state,
                e.poll AS poll,
                e.participant_ids AS participantids,
                e.name AS name
            FROM random_coffee.entries AS e
            WHERE e.id = @id;

            SELECT
                h.id AS id,
                h.created AS created,
                h.random_coffee_entry_id AS randomcoffeeentryid,
                h.pairs AS pairs,
                h.excluded_person_id AS excludedpersonid
            FROM random_coffee.history AS h
            WHERE h.random_coffee_entry_id = @id
            ORDER BY h.created DESC
            OFFSET 0
            LIMIT @history_depth;
            """,
            new
            {
                id = id,
                history_depth = historyDepth
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var query = await connection.QueryMultipleAsync(command);
        
        var randomCoffeeEntry = await query.ReadSingleOrDefaultAsync<RandomCoffeeEntry>();
        var randomCoffeeHistory = await query.ReadAsync<RandomCoffeeHistory>();
        
        if (randomCoffeeEntry is not null)
            foreach (var item in randomCoffeeHistory)
                randomCoffeeEntry.AddHistory(item);

        return randomCoffeeEntry;
    }
}