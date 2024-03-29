using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.MarkMessageForDelete;

internal sealed class MarkMessageForDeleteCommandHandler : IRequestHandler<MarkMessageForDeleteCommand, CommandResult>
{
    private readonly DialogContinuation _dialogContinuation;

    public MarkMessageForDeleteCommandHandler(DialogContinuation dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<CommandResult> Handle(MarkMessageForDeleteCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var dialogState = _dialogContinuation.Find(command.MainContext.PersonId);
        dialogState?.Attach(new ChatMessage(command.MainContext.ChatId, command.MessageId));
        
        return Task.FromResult(CommandResult.Empty);
    }
}