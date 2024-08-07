using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class SingleLineCommandFactory
{
    private readonly CommandCreatorResolver _commandCreatorResolver;
    private readonly DialogContinuation _dialogContinuation;
    
    public SingleLineCommandFactory(
        CommandCreatorResolver commandCreatorResolver,
        DialogContinuation dialogContinuation)
    {
        _commandCreatorResolver =
            commandCreatorResolver ?? throw new ArgumentNullException(nameof(commandCreatorResolver));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }
    
    public async Task<IEndDialogCommand?> TryCreate(
        Bot bot,
        MessageContext messageContext,
        string input,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(input));
        
        const char cmdSeparator = ' ';
        const int minParametersCount = 3;
        var parameters = input.Split(cmdSeparator).ToArray();

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
        var command = await commandCreator.Create(
            messageContext with { Text = description },
            teamContext,
            token);
        
        _dialogContinuation.Begin(
            messageContext.Bot.Id,
            messageContext.TargetChat,
            cmd,
            CommandStage.None,
            messageContext.ChatMessage);
        
        return command;

        bool TeamFilter(string name) => name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase);
    }
}