using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.End;

internal sealed class EndCommandHandler : IRequestHandler<EndCommand, CommandResult>
{
    private readonly DialogContinuation _dialogContinuation;

    public EndCommandHandler(DialogContinuation dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }
    
    public Task<CommandResult> Handle(EndCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        if (_dialogContinuation.Find(command.MessageContext.PersonId) is null)
            _dialogContinuation.Begin(
                command.MessageContext.PersonId,
                CommandList.Cancel,
                CommandStage.None,
                new ChatMessage(command.MessageContext.ChatId, command.MessageContext.MessageId));
            
        return Task.FromResult(CommandResult.Empty);
    }
}