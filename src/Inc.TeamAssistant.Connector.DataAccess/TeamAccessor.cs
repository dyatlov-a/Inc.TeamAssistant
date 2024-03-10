using Dapper;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Npgsql;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class TeamAccessor : ITeamAccessor
{
    private readonly string _connectionString;

    public TeamAccessor(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }
    
    public async Task<IReadOnlyCollection<(long PersonId, string PersonDisplayName)>> GetTeammates(
        Guid teamId,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.language_id AS languageid,
                p.username AS username
            FROM connector.persons AS p
            JOIN connector.teammates AS tm ON p.id = tm.person_id
            WHERE tm.team_id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        var persons = await connection.QueryAsync<Person>(command);
        return persons.Select(p => (p.Id, p.DisplayName)).ToArray();
    }

    public async Task<(long Id, string Name, string? Username, string PersonDisplayName, LanguageId LanguageId)?> FindPerson(
        long userId,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username,
                COALESCE(p.username, p.name) AS persondisplayname,
                p.language_id AS languageid
            FROM connector.persons AS p
            WHERE p.id = @id;",
            new { id = userId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection
            .QuerySingleOrDefaultAsync<(long Id, string Name, string? Username, string PersonDisplayName, LanguageId LanguageId)?>(command);
    }
}