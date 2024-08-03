using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.End;
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

        if (messageContext.Text.StartsWith(CommandList.Cancel, StringComparison.InvariantCultureIgnoreCase))
            return new EndCommand(messageContext);

        var dialogState = _dialogContinuation.Find(bot.Id, messageContext.TargetChat);
        var input = dialogState is null ? _aliasService.OverrideCommand(messageContext.Text) : dialogState.Command;
        
        var botCommand = bot.FindCommand(input);
        if (botCommand is null)
            return null;

        var singleLineCommand = await _singleLineCommandFactory.TryCreate(bot, messageContext, input, token);
        if (singleLineCommand is not null)
            return singleLineCommand;
        
        var currentNode = botCommand.Stages.First;
        while (currentNode is not null)
        {
            if ((dialogState is null && botCommand.Stages.First!.Value.Id == currentNode.Value.Id) ||
                dialogState?.State == currentNode.Value.Value)
            {
                var dialogCommand = await _dialogCommandFactory.TryCreate(
                    bot,
                    botCommand.Value,
                    dialogState?.State,
                    currentNode.Value,
                    currentNode.Next?.Value,
                    messageContext);

                if (dialogCommand is not null)
                    return dialogCommand;
            }

            currentNode = currentNode.Next;
        }

        var commandCreator = _commandCreatorResolver.TryResolve(input);
        if (commandCreator is null)
            return null;
        
        var currentTeamContext = dialogState?.TeamContext ?? CurrentTeamContext.Empty;
        var command = await commandCreator.Create(messageContext, currentTeamContext, token);
        return command;
    }
}