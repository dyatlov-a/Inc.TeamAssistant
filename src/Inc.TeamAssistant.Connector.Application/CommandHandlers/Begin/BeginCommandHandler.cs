using Inc.TeamAssistant.Connector.Model.Commands.Begin;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin;

internal sealed class BeginCommandHandler : IRequestHandler<BeginCommand, CommandResult>
{
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;

    public BeginCommandHandler(IDialogContinuation<BotCommandStage> dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<CommandResult> Handle(BeginCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var dialogState = _dialogContinuation.TryBegin(command.MessageContext.PersonId, command.NextStage);

        if (dialogState is null)
            throw new ApplicationException("Can not start command dialog.");

        dialogState.AddItem(command.Command);
        
        if (command.MessageContext.Shared)
            dialogState.TryAttachMessage(new ChatMessage(
                command.MessageContext.ChatId,
                command.MessageContext.MessageId));

        return Task.FromResult(CommandResult.Build(command.Notification));
    }
}