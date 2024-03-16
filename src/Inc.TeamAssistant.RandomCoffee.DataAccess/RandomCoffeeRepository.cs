using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Npgsql;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

internal sealed class RandomCoffeeRepository : IRandomCoffeeRepository
{
    private readonly string _connectionString;

    public RandomCoffeeRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }

    public async Task<RandomCoffeeEntry?> Find(Guid id, CancellationToken token)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        return await Find(connection, id, token);
    }

    public async Task<RandomCoffeeEntry?> Find(string pollId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT e.id AS id
            FROM random_coffee.entries AS e
            WHERE e.poll_id = @poll_id;",
            new { poll_id = pollId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);

        var randomCoffeeEntryId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);

        return randomCoffeeEntryId.HasValue
            ? await Find(connection, randomCoffeeEntryId.Value, token)
            : null;
    }

    public async Task<RandomCoffeeEntry?> Find(long chatId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT e.id AS id
            FROM random_coffee.entries AS e
            WHERE e.chat_id = @chat_id;",
            new { chat_id = chatId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);

        var randomCoffeeEntryId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);

        return randomCoffeeEntryId.HasValue
            ? await Find(connection, randomCoffeeEntryId.Value, token)
            : null;
    }

    public async Task Upsert(RandomCoffeeEntry randomCoffeeEntry, CancellationToken token)
    {
        if (randomCoffeeEntry is null)
            throw new ArgumentNullException(nameof(randomCoffeeEntry));

        var historyId = new List<Guid>(randomCoffeeEntry.History.Count);
        var historyCreated = new List<DateTimeOffset>(randomCoffeeEntry.History.Count);
        var historyPairs = new List<string>(randomCoffeeEntry.History.Count);
        var excludedPersonId = new List<long?>(randomCoffeeEntry.History.Count);

        foreach (var item in randomCoffeeEntry.History)
        {
            historyId.Add(item.Id);
            historyCreated.Add(item.Created);
            historyPairs.Add(JsonSerializer.Serialize(item.Pairs));
            excludedPersonId.Add(item.ExcludedPersonId);
        }

        var upsertEntry = new CommandDefinition(@"
            INSERT INTO random_coffee.entries (
                id,
                created,
                chat_id,
                next_round,
                state,
                poll_id,
                participant_ids)
            VALUES (
                @id,
                @created,
                @chat_id,
                @next_round,
                @state,
                @poll_id,
                @participant_ids::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                created = excluded.created,
                chat_id = excluded.chat_id,
                next_round = excluded.next_round,
                state = excluded.state,
                poll_id = excluded.poll_id,
                participant_ids = excluded.participant_ids;",
            new
            {
                id = randomCoffeeEntry.Id,
                created = randomCoffeeEntry.Created,
                chat_id = randomCoffeeEntry.ChatId,
                next_round = randomCoffeeEntry.NextRound,
                state = randomCoffeeEntry.State,
                poll_id = randomCoffeeEntry.PollId,
                participant_ids = JsonSerializer.Serialize(randomCoffeeEntry.ParticipantIds)
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await using var transaction = await connection.BeginTransactionAsync(token);

        await connection.ExecuteAsync(upsertEntry);

        if (randomCoffeeEntry.History.Any())
        {
            var upsertHistory = new CommandDefinition(@"
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
                    excluded_person_id = excluded.excluded_person_id;",
                new
                {
                    id = historyId,
                    created = historyCreated,
                    random_coffee_entry_id = randomCoffeeEntry.Id,
                    pairs = historyPairs,
                    excluded_person_id = excludedPersonId
                },
                flags: CommandFlags.None,
                cancellationToken: token);
            
            await connection.ExecuteAsync(upsertHistory);
        }

        await transaction.CommitAsync(token);
    }
    
    private async Task<RandomCoffeeEntry?> Find(NpgsqlConnection connection, Guid id, CancellationToken token)
    {
        const int historyDepth = 5;
        var command = new CommandDefinition(@"
            SELECT
                e.id AS id,
                e.created AS created,
                e.bot_id AS botid,
                e.chat_id AS chatid,
                e.next_round AS nextround,
                e.state AS state,
                e.poll_id AS pollid,
                e.participant_ids AS participantids
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
            LIMIT @history_depth;",
            new
            {
                id,
                history_depth = historyDepth
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var query = await connection.QueryMultipleAsync(command);
        
        var randomCoffeeEntry = await query.ReadSingleOrDefaultAsync<RandomCoffeeEntry>();
        var randomCoffeeHistory = await query.ReadAsync<RandomCoffeeHistory>();
        
        if (randomCoffeeEntry is not null)
            foreach (var item in randomCoffeeHistory)
                randomCoffeeEntry.AddHistory(item);

        return randomCoffeeEntry;
    }
}