using Inc.TeamAssistant.Connector.Application.Alias;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandFactory
{
    private readonly CommandCreatorResolver _commandCreatorResolver;
    private readonly DialogContinuation _dialogContinuation;
    private readonly DialogCommandFactory _dialogCommandFactory;
    private readonly SingleLineCommandFactory _singleLineCommandFactory;
    private readonly AliasService _aliasService;

    public CommandFactory(
        CommandCreatorResolver commandCreatorResolver,
        DialogContinuation dialogContinuation,
        DialogCommandFactory dialogCommandFactory,
        SingleLineCommandFactory singleLineCommandFactory,
        AliasService aliasService)
    {
        _commandCreatorResolver =
            commandCreatorResolver ?? throw new ArgumentNullException(nameof(commandCreatorResolver));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _dialogCommandFactory = dialogCommandFactory ?? throw new ArgumentNullException(nameof(dialogCommandFactory));
        _aliasService = aliasService ?? throw new ArgumentNullException(nameof(aliasService));
        _singleLineCommandFactory =
            singleLineCommandFactory ?? throw new ArgumentNullException(nameof(singleLineCommandFactory));
    }

    public async Task<IDialogCommand?> TryCreate(Bot bot, MessageContext messageContext, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);

        var inputCommand = _aliasService.OverrideCommand(messageContext.Text);
        var simpleCommand = await TryCreateSimple(inputCommand, bot, messageContext, token);
        if (simpleCommand is not null)
            return simpleCommand;
        
        var singleLineCommand = await _singleLineCommandFactory.TryCreate(bot, messageContext, inputCommand, token);
        if (singleLineCommand is not null)
            return singleLineCommand;
        
        var dialogState = _dialogContinuation.Find(bot.Id, messageContext.TargetChat);
        var contextCommand = dialogState is null ? inputCommand : dialogState.Command;
        var botCommand = bot.FindCommand(contextCommand);
        if (botCommand is null)
            return null;
        
        var dialogCommand = TryCreateDialogCommand(botCommand, bot, messageContext, dialogState);
        if (dialogCommand is not null)
            return dialogCommand;

        var commandCreator = _commandCreatorResolver.TryResolve(contextCommand);
        if (commandCreator is null)
            return null;
        
        var currentTeamContext = dialogState?.TeamContext ?? CurrentTeamContext.Empty;
        var command = await commandCreator.Create(messageContext, currentTeamContext, token);
        return command;
    }

    private async Task<IDialogCommand?> TryCreateSimple(
        string inputCommand,
        Bot bot,
        MessageContext messageContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(inputCommand);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        var simpleCommand = bot.FindCommand(inputCommand);
        
        if (simpleCommand?.MultipleStages == false)
        {
            var priorityCommandCreator = _commandCreatorResolver.TryResolve(inputCommand);
            
            if (priorityCommandCreator is not null)
                return await priorityCommandCreator.Create(messageContext, CurrentTeamContext.Empty, token);
        }

        return null;
    }

    private IDialogCommand? TryCreateDialogCommand(
        ContextCommand botCommand,
        Bot bot,
        MessageContext messageContext,
        DialogState? dialogState)
    {
        ArgumentNullException.ThrowIfNull(botCommand);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        while (botCommand.Stages.MoveNext())
        {
            if ((dialogState is null && botCommand.Stages.IsFirst) || dialogState?.State == botCommand.Stages.Current.Value)
            {
                var dialogCommand = _dialogCommandFactory.TryCreate(
                    bot,
                    botCommand.Value,
                    dialogState?.State,
                    botCommand.Stages.Current,
                    botCommand.Stages.Next,
                    messageContext);

                if (dialogCommand is not null)
                    return dialogCommand;
            }
        }

        return null;
    }
}