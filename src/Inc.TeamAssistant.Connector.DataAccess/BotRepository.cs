using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Npgsql;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class BotRepository : IBotRepository
{
    private readonly string _connectionString;

    public BotRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }
    
    public async Task<IReadOnlyCollection<Bot>> GetAll(CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                b.id AS id,
                b.name AS name,
                b.token AS token
            FROM connector.bots AS b;",
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<Bot>(command);
        return results.ToArray();
    }

    public async Task<Bot?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                b.id AS id,
                b.name AS name,
                b.token AS token
            FROM connector.bots AS b
            WHERE b.id = @id;

            SELECT
                bc.id AS id,
                bc.value AS value,
                bc.help_message_id AS helpmessageid
            FROM connector.bot_commands AS bc
            WHERE bc.bot_id = @id;

            SELECT
                bcs.id AS id,
                bcs.bot_command_id AS botcommandid,
                bcs.value AS value,
                bcs.dialog_message_id AS dialogmessageid,
                bcs.position AS position
            FROM connector.bot_command_stages AS bcs
            JOIN connector.bot_commands AS bc ON bc.id = bcs.bot_command_id
            WHERE bc.bot_id = @id;

            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.chat_id AS chatid,
                t.name AS name
            FROM connector.teams AS t
            WHERE t.bot_id = @id;

            SELECT
                p.id AS id,
                p.name AS name,
                p.language_id AS languageid,
                p.username AS username,
                tm.team_id AS teamid
            FROM connector.persons AS p
            JOIN connector.teammates AS tm ON p.id = tm.person_id
            JOIN connector.teams AS t ON t.id = tm.team_id
            WHERE t.bot_id = @id;",
            new { id },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var query = await connection.QueryMultipleAsync(command);
        
        var bot = await query.ReadSingleOrDefaultAsync<Bot>();
        var botCommands = await query.ReadAsync<BotCommand>();
        var botCommandStages = (await query.ReadAsync<BotCommandStage>()).ToLookup(s => s.BotCommandId);
        var teams = await query.ReadAsync<Team>();
        var personsLookup = (await query.ReadAsync<(
                long Id,
                string Name,
                LanguageId LanguageId,
                string? Username,
                Guid TeamId)>())
            .ToLookup(p => p.TeamId);

        if (bot is not null)
        {
            foreach (var botCommand in botCommands)
            {
                foreach (var botCommandStage in botCommandStages[botCommand.Id])
                    botCommand.AddStage(botCommandStage);
                
                bot.AddCommand(botCommand);
            }

            foreach (var team in teams)
            {
                foreach (var person in personsLookup[team.Id])
                    team.AddTeammate(new Person(person.Id, person.Name, person.LanguageId, person.Username));
                
                bot.AddTeam(team);
            }
        }

        return bot;
    }
}