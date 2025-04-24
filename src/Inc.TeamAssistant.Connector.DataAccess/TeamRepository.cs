using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class TeamRepository : ITeamRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public TeamRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Team?> Find(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.chat_id AS chatid,
                t.name AS name,
                t.properties AS properties,
                p.id AS ownerid,
                p.name AS ownername,
                p.username AS ownerusername
            FROM connector.teams AS t
            JOIN connector.persons AS p ON p.id = t.owner_id
            WHERE t.id = @team_id;

            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            JOIN connector.teammates AS tm ON p.id = tm.person_id
            WHERE tm.team_id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        await using var query = await connection.QueryMultipleAsync(command);

        var teamDb = await query.ReadSingleOrDefaultAsync<TeamDb>();
        var persons = await query.ReadAsync<Person>();

        if (teamDb is not null)
        {
            var team = new Team(
                teamDb.Id,
                teamDb.BotId,
                teamDb.ChatId,
                new Person(teamDb.OwnerId, teamDb.OwnerName, teamDb.OwnerUsername),
                teamDb.Name,
                teamDb.Properties);
            
            foreach (var person in persons)
                team.AddTeammate(person);

            return team;
        }

        return null;
    }

    public async Task Upsert(Team team, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(team);

        var upsertTeam = new CommandDefinition(@"
            INSERT INTO connector.teams (id, bot_id, chat_id, owner_id, name, properties)
            VALUES (@id, @bot_id, @chat_id, @owner_id, @name, @properties::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                bot_id = EXCLUDED.bot_id,
                chat_id = EXCLUDED.chat_id,
                owner_id = EXCLUDED.owner_id,
                name = EXCLUDED.name,
                properties = EXCLUDED.properties;",
            new
            {
                id = team.Id,
                bot_id = team.BotId,
                chat_id = team.ChatId,
                owner_id = team.Owner.Id,
                name = team.Name,
                properties = JsonSerializer.Serialize(team.Properties)
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        var personIds = team.Teammates.Select(t => t.Id).ToArray();
        var upsertTeammates = new CommandDefinition(@"
            MERGE INTO connector.teammates AS ttm
            USING (
                SELECT DISTINCT ON (q.person_id)
                    @team_id AS team_id,
                    q.person_id AS person_id,
                    q.mark_as_removed AS mark_as_removed
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
                INSERT VALUES(stm.team_id, stm.person_id, NULL, false)
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

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertTeam);
        await connection.ExecuteAsync(upsertTeammates);

        await transaction.CommitAsync(token);
    }

    public async Task Remove(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(
            "DELETE FROM connector.teams WHERE id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
    
    public async Task<bool> HasManagerAccess(Guid teamId, long personId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT true
            FROM connector.teams AS t
            JOIN connector.bots AS b ON t.bot_id = b.id
            WHERE t.id = @team_id AND (t.owner_id = @person_id OR b.owner_id = @person_id)
            LIMIT 1;
            """,
            new
            {
                team_id = teamId,
                person_id = personId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var hasManagerAccess = await connection.QuerySingleOrDefaultAsync<bool>(command);
        
        return hasManagerAccess;
    }
}