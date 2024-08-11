using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class SingleLineCommandFactory
{
    private readonly CommandCreatorResolver _commandCreatorResolver;
    
    public SingleLineCommandFactory(CommandCreatorResolver commandCreatorResolver)
    {
        _commandCreatorResolver =
            commandCreatorResolver ?? throw new ArgumentNullException(nameof(commandCreatorResolver));
    }
    
    public async Task<IEndDialogCommand?> TryCreate(
        Bot bot,
        MessageContext messageContext,
        string inputCommand,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputCommand);
        
        const char cmdSeparator = ' ';
        const int minParametersCount = 3;
        var parameters = inputCommand.Split(cmdSeparator).ToArray();

        if (parameters.Length < minParametersCount)
            return null;

        var cmd = parameters[0];
        var teamName = parameters[1];
        var description = string.Join(cmdSeparator, parameters[2..]);

        var commandCreator = _commandCreatorResolver.TryResolve(cmd, onlySingleLineCommand: true);
        var canSelectTeam = messageContext.Teams.Any(t => TeamFilter(t.Name));
        var teamSettings = bot.Teams.SingleOrDefault(t => TeamFilter(t.Name));

        if (!canSelectTeam || commandCreator is null || teamSettings is null)
            return null;
        
        var teamContext = new CurrentTeamContext(teamSettings.Id, teamSettings.Properties);
        return await commandCreator.Create(
            messageContext with { Text = description },
            teamContext,
            token);

        bool TeamFilter(string name) => name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase);
    }
}