using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.End;

internal sealed class EndCommandHandler : IRequestHandler<EndCommand, CommandResult>
{
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;

    public EndCommandHandler(IDialogContinuation<BotCommandStage> dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }
    
    public Task<CommandResult> Handle(EndCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var dialogState = _dialogContinuation.TryEnd(command.MessageContext.PersonId, command.CurrentStage);

        if (dialogState is not null)
        {
            if (command.MessageContext.Shared)
                dialogState.TryAttachMessage(new ChatMessage(
                    command.MessageContext.ChatId,
                    command.MessageContext.MessageId));
            
            if (dialogState.ChatMessages.Any())
                return Task.FromResult(CommandResult.Build(NotificationMessage.Delete(dialogState.ChatMessages)));
        }
            
        return Task.FromResult(CommandResult.Empty);
    }
}