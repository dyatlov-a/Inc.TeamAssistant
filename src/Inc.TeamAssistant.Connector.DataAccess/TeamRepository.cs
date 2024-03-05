using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class TeamRepository : ITeamRepository
{
    private readonly string _connectionString;

    public TeamRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }

    public async Task<Team?> Find(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.chat_id AS chatid,
                t.name AS name,
                t.properties AS properties
            FROM connector.teams AS t
            WHERE t.id = @team_id;

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
        
        var query = await connection.QueryMultipleAsync(command);

        var team = await query.ReadSingleOrDefaultAsync<Team>();
        var persons = await query.ReadAsync<Person>();

        if (team is not null)
            foreach (var person in persons)
                team.AddTeammate(person);

        return team;
    }

    public async Task Upsert(Team team, CancellationToken token)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));
        
        var upsertTeam = new CommandDefinition(@"
            INSERT INTO connector.teams (id, bot_id, chat_id, name, properties)
            VALUES (@id, @bot_id, @chat_id, @name, @properties::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                bot_id = EXCLUDED.bot_id,
                chat_id = EXCLUDED.chat_id,
                name = EXCLUDED.name,
                properties = EXCLUDED.properties;",
            new
            {
                id = team.Id,
                bot_id = team.BotId,
                chat_id = team.ChatId,
                name = team.Name,
                properties = JsonSerializer.Serialize(team.Properties)
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        var personIds = team.Teammates.Select(t => t.Id).ToArray();
        var upsertTeammates = new CommandDefinition(@"
            MERGE INTO connector.teammates AS ttm
            USING (
                SELECT DISTINCT ON (q.person_id) @team_id AS team_id, q.person_id AS person_id, q.mark_as_removed
                FROM (
                    SELECT tm.person_id, false AS mark_as_removed
                    FROM UNNEST(@person_ids) AS tm(person_id)
                    UNION
                    SELECT tm.person_id, true AS mark_as_removed
                    FROM connector.teammates AS tm
                    WHERE tm.team_id = @team_id) AS q
                ORDER BY q.person_id, q.mark_as_removed
            ) AS stm ON ttm.team_id = stm.team_id AND ttm.person_id = stm.person_id
            WHEN NOT MATCHED THEN
                INSERT VALUES(stm.team_id, stm.person_id)
            WHEN MATCHED AND NOT stm.mark_as_removed THEN
                UPDATE SET team_id = stm.team_id, person_id = stm.person_id
            WHEN MATCHED THEN
                DELETE;",
            new
            {
                person_ids = personIds,
                team_id = team.Id
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertTeam);
        await connection.ExecuteAsync(upsertTeammates);

        await transaction.CommitAsync(token);
    }
}