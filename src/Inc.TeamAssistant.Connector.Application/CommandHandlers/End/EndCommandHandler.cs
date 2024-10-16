using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.Primitives.Commands;
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
        ArgumentNullException.ThrowIfNull(command);

        if (_dialogContinuation.Find(command.MessageContext.Bot.Id, command.MessageContext.TargetChat) is null)
            _dialogContinuation.Begin(
                command.MessageContext.Bot.Id,
                command.MessageContext.TargetChat,
                CommandList.Cancel,
                StageType.None,
                command.MessageContext.ChatMessage);
            
        return Task.FromResult(CommandResult.Empty);
    }
}