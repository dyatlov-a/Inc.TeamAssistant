using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;
using Inc.TeamAssistant.Primitives.Commands;
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
        ArgumentNullException.ThrowIfNull(command);

        var dialogState = _dialogContinuation.Begin(
            command.MessageContext.Bot.Id,
            command.MessageContext.TargetChat,
            command.Command,
            command.NextStage,
            command.MessageContext.ChatMessage);

        if (command.TeamContext != CurrentTeamContext.Empty)
            dialogState.SetTeam(command.TeamContext);

        command.Notification.WithHandler((c, p) => new MarkMessageForDeleteCommand(c, int.Parse(p)));

        return Task.FromResult(CommandResult.Build(command.Notification));
    }
}