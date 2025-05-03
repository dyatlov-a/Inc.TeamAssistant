using Inc.TeamAssistant.Connector.Application.Alias;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;

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

        var input = _aliasService.OverrideCommand(messageContext.Text);
        var inputCommand = bot.FindCommand(input);
        if (inputCommand is not null)
        {
            var singleLineCommand = _singleLineCommandFactory.TryCreate(bot, messageContext, input);
            if (singleLineCommand is not null)
                return singleLineCommand;

            var singleStageCommand = TryCreateSingleStageCommand(inputCommand, bot, messageContext);
            if (singleStageCommand is not null)
                return singleStageCommand;
        }
            
        var dialogState = _dialogContinuation.Find(bot.Id, messageContext.TargetChat);
        var dialogInput = dialogState is null ? input : dialogState.Command;
        var dialogCommand = bot.FindCommand(dialogInput);
        if (dialogCommand is null || !dialogInput.HasCommand())
            return _commandCreatorFactory.TryCreate(
                messageContext.Text,
                singleLineMode: false,
                messageContext,
                CurrentTeamContext.Empty);
        
        var multiStageCommand = TryCreateMultiStageCommand(dialogCommand, bot, messageContext, dialogState);
        if (multiStageCommand is not null)
            return multiStageCommand;
        
        return _commandCreatorFactory.TryCreate(
            dialogInput,
            singleLineMode: false,
            messageContext,
            dialogState?.TeamContext ?? CurrentTeamContext.Empty);
    }
    
    private IDialogCommand? TryCreateSingleStageCommand(ContextCommand command, Bot bot, MessageContext messageContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);

        return command.MultipleStages
            ? null
            : _commandCreatorFactory.TryCreate(
                command.Value,
                singleLineMode: false,
                messageContext,
                CurrentTeamContext.Empty);
    }

    private IDialogCommand? TryCreateMultiStageCommand(
        ContextCommand command,
        Bot bot,
        MessageContext messageContext,
        DialogState? dialogState)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        while (command.Stages.MoveNext())
        {
            var isFirst = dialogState is null && command.Stages.IsFirst;
            var isCurrent = dialogState?.State == command.Stages.Current.Value;
            
            if (isFirst || isCurrent)
            {
                var dialogCommand = _dialogCommandFactory.TryCreate(
                    bot,
                    command.Value,
                    dialogState?.State,
                    command.Stages.Current,
                    command.Stages.Next,
                    messageContext);

                if (dialogCommand is not null)
                    return dialogCommand;
            }
        }

        return null;
    }
}