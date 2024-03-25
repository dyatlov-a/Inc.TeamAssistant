using Dapper;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Npgsql;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

internal sealed class RandomCoffeeReader : IRandomCoffeeReader
{
    private readonly string _connectionString;

    public RandomCoffeeReader(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }
    
    public async Task<IReadOnlyCollection<RandomCoffeeEntry>> GetByDate(DateOnly date, CancellationToken token)
    {
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
                e.participant_ids AS participantids
            FROM random_coffee.entries AS e
            WHERE e.next_round = @date;",
            new { date },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<RandomCoffeeEntry>(command);
        return results.ToArray();
    }
}