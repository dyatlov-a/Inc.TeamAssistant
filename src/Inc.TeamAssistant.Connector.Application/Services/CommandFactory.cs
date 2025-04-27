using Inc.TeamAssistant.Connector.Application.Alias;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandFactory
{
    private readonly CommandCreatorFactory _commandCreatorFactory;
    private readonly DialogContinuation _dialogContinuation;
    private readonly DialogCommandFactory _dialogCommandFactory;
    private readonly SingleLineCommandFactory _singleLineCommandFactory;
    private readonly AliasService _aliasService;

    public CommandFactory(
        CommandCreatorFactory commandCreatorFactory,
        DialogContinuation dialogContinuation,
        DialogCommandFactory dialogCommandFactory,
        SingleLineCommandFactory singleLineCommandFactory,
        AliasService aliasService)
    {
        _commandCreatorFactory =
            commandCreatorFactory ?? throw new ArgumentNullException(nameof(commandCreatorFactory));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _dialogCommandFactory = dialogCommandFactory ?? throw new ArgumentNullException(nameof(dialogCommandFactory));
        _aliasService = aliasService ?? throw new ArgumentNullException(nameof(aliasService));
        _singleLineCommandFactory =
            singleLineCommandFactory ?? throw new ArgumentNullException(nameof(singleLineCommandFactory));
    }

    public IDialogCommand? TryCreate(Bot bot, MessageContext messageContext)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);

        var inputCommand = _aliasService.OverrideCommand(messageContext.Text);
        var simpleCommand = TryCreateSimple(inputCommand, bot, messageContext);
        if (simpleCommand is not null)
            return simpleCommand;
        
        var singleLineCommand = _singleLineCommandFactory.TryCreate(bot, messageContext, inputCommand);
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
        
        var currentTeamContext = dialogState?.TeamContext ?? CurrentTeamContext.Empty;
        var command = _commandCreatorFactory.TryCreate(
            contextCommand,
            singleLineMode: false,
            messageContext,
            currentTeamContext);
        return command;
    }

    private IDialogCommand? TryCreateSimple(string inputCommand, Bot bot, MessageContext messageContext)
    {
        ArgumentNullException.ThrowIfNull(inputCommand);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        var simpleCommand = bot.FindCommand(inputCommand);
        
        if (simpleCommand?.MultipleStages == false)
        {
            var command = _commandCreatorFactory.TryCreate(
                inputCommand,
                singleLineMode: false,
                messageContext,
                CurrentTeamContext.Empty);
            return command;
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