using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class BotReader : IBotReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public BotReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<Guid>> GetBotIds(CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT b.id AS id
            FROM connector.bots AS b;",
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<Guid>(command);
        return results.ToArray();
    }

    public async Task<IReadOnlyCollection<BotDto>> GetBotsByUser(long userId, CancellationToken token)
    {
        var botsCommand = new CommandDefinition(@"
            SELECT DISTINCT b.id AS id, b.name AS name
            FROM connector.bots AS b
            LEFT JOIN connector.teams AS t ON t.bot_id = b.id
            LEFT JOIN connector.teammates AS tm ON t.id = tm.team_id
            WHERE t.owner_id = @user_id OR tm.person_id = @user_id OR b.owner_id = @user_id;",
            new { user_id = userId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var bots = (await connection.QueryAsync<(Guid BotId, string Name)>(botsCommand)).ToArray();
        var botIds = bots.Select(b => b.BotId).ToArray();
        
        var dataCommand = new CommandDefinition(@"
            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.chat_id AS chatid,
                t.owner_id AS ownerid,
                t.name AS name,
                t.properties AS properties
            FROM connector.teams AS t
            WHERE t.bot_id = ANY(@bot_ids);

            SELECT
                af.bot_id AS botid,
                f.name AS featurename
            FROM connector.features AS f
            JOIN connector.activated_features AS af ON f.id = af.feature_id
            WHERE af.bot_id = ANY(@bot_ids);",
            new { bot_ids = botIds },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var query = await connection.QueryMultipleAsync(dataCommand);
        
        var teams = (await query.ReadAsync<Team>()).ToLookup(t => t.BotId);
        var features = (await query.ReadAsync<(Guid BotId, string FeatureName)>())
            .ToLookup(f => f.BotId, f => f.FeatureName);
        var results = bots
            .Select(b => new BotDto(
                b.BotId,
                b.Name,
                features[b.BotId].ToArray(),
                teams[b.BotId].Select(t => new TeamDto(t.Id, t.Name)).ToArray()))
            .ToArray();
        
        return results;
    }

    public async Task<Bot?> Find(Guid id, DateTimeOffset now, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                b.id AS id,
                b.name AS name,
                b.token AS token,
                b.properties AS properties
            FROM connector.bots AS b
            WHERE b.id = @id;

            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.chat_id AS chatid,
                t.owner_id AS ownerid,
                t.name AS name,
                t.properties AS properties
            FROM connector.teams AS t
            WHERE t.bot_id = @id;

            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username,
                tm.team_id AS teamid
            FROM connector.persons AS p
            JOIN connector.teammates AS tm ON p.id = tm.person_id AND (tm.leave_until IS NULL OR tm.leave_until < @now)
            JOIN connector.teams AS t ON t.id = tm.team_id
            WHERE t.bot_id = @id;

            SELECT DISTINCT cp.bot_command_id
            FROM connector.activated_features AS af
            JOIN connector.features AS f ON af.feature_id = f.id
            JOIN connector.command_packs AS cp ON cp.feature_id = f.id
            WHERE af.bot_id = @id;

            SELECT
                bc.id AS id,
                bc.value AS value,
                bc.help_message_id AS helpmessageid,
                bc.scopes AS scopes
            FROM connector.bot_commands AS bc;

            SELECT
                bcs.id AS id,
                bcs.bot_command_id AS botcommandid,
                bcs.value AS value,
                bcs.dialog_message_id AS dialogmessageid,
                bcs.position AS position
            FROM connector.bot_command_stages AS bcs;",
            new
            {
                id,
                now
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await using var query = await connection.QueryMultipleAsync(command);
        
        var bot = await query.ReadSingleOrDefaultAsync<Bot>();
        var teams = await query.ReadAsync<Team>();
        var personsLookup = (await query.ReadAsync<(long Id, string Name, string? Username, Guid TeamId)>())
            .ToLookup(p => p.TeamId);
        var commandIds = await query.ReadAsync<Guid>();
        var botCommands = (await query.ReadAsync<BotCommand>()).ToDictionary(s => s.Id);
        var botCommandStages = (await query.ReadAsync<BotCommandStage>()).ToLookup(s => s.BotCommandId);

        if (bot is not null)
        {
            foreach (var botCommand in commandIds.Select(i => botCommands[i]))
            {
                foreach (var botCommandStage in botCommandStages[botCommand.Id])
                    botCommand.AddStage(botCommandStage);
                
                bot.AddCommand(botCommand);
            }

            foreach (var team in teams)
            {
                foreach (var person in personsLookup[team.Id])
                    team.AddTeammate(new Person(person.Id, person.Name, person.Username));
                
                bot.AddTeam(team);
            }
        }

        return bot;
    }

    public async Task<string> GetToken(Guid botId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT b.token AS token
            FROM connector.bots AS b
            WHERE b.id = @bot_id;",
            new { bot_id = botId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var botToken = await connection.QuerySingleAsync<string>(command);
        return botToken;
    }
}