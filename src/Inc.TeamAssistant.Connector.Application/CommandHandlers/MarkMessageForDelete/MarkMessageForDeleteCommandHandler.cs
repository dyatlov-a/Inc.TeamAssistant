using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;
using Inc.TeamAssistant.Primitives.Commands;
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
        ArgumentNullException.ThrowIfNull(command);

        var dialogState = _dialogContinuation.Find(command.MainContext.TargetChat);
        dialogState?.Attach(command.MainContext.ChatMessage with { MessageId = command.MessageId });
        
        return Task.FromResult(CommandResult.Empty);
    }
}