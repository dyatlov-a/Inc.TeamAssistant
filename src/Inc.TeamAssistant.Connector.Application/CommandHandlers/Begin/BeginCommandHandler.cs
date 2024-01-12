using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.Begin;
using Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin;

internal sealed class BeginCommandHandler : IRequestHandler<BeginCommand, CommandResult>
{
    private readonly DialogContinuation _dialogContinuation;

    public BeginCommandHandler(DialogContinuation dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<CommandResult> Handle(BeginCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var dialogState = _dialogContinuation.Begin(
            command.MessageContext.PersonId,
            command.Command,
            (CommandStage)command.NextStage,
            new ChatMessage(
                command.MessageContext.ChatId,
                command.MessageContext.MessageId,
                command.MessageContext.Shared));

        if (command.SelectedTeamId.HasValue)
            dialogState.SetTeamId(command.SelectedTeamId.Value);

        command.Notification.AddHandler((c, id) => new MarkMessageForDeleteCommand(c, id));

        return Task.FromResult(CommandResult.Build(command.Notification));
    }
}