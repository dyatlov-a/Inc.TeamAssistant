using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam;

internal sealed class LeaveFromTeamCommandHandler : IRequestHandler<LeaveFromTeamCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IEnumerable<ILeaveTeamHandler> _leaveTeamHandlers;

    public LeaveFromTeamCommandHandler(
        ITeamRepository teamRepository,
        IDialogContinuation<BotCommandStage> dialogContinuation,
        IMessageBuilder messageBuilder,
        IEnumerable<ILeaveTeamHandler> leaveTeamHandlers)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _leaveTeamHandlers = leaveTeamHandlers ?? throw new ArgumentNullException(nameof(leaveTeamHandlers));
    }
    
    public async Task<CommandResult> Handle(LeaveFromTeamCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new ApplicationException($"Team {command.TeamId} was not found.");

        team.RemoveTeammate(command.MessageContext.PersonId);

        await _teamRepository.Upsert(team, token);

        foreach (var leaveTeamHandler in _leaveTeamHandlers)
            await leaveTeamHandler.Handle(command.MessageContext, command.TeamId, token);

        var leaveFromTeamMessage = await _messageBuilder.Build(
            Messages.Connector_LeaveTeamSuccess,
            command.MessageContext.LanguageId,
            command.MessageContext.DisplayUsername,
            team.Name);
        var notifications = new List<NotificationMessage>
        {
            NotificationMessage.Create(command.MessageContext.ChatId, leaveFromTeamMessage)
        };
        
        var dialogState = _dialogContinuation.TryEnd(command.MessageContext.PersonId, BotCommandStage.SelectTeam);
        if (dialogState is not null)
        {
            if (command.MessageContext.Shared)
                dialogState.TryAttachMessage(new ChatMessage(
                    command.MessageContext.ChatId,
                    command.MessageContext.MessageId));
            
            if (dialogState.ChatMessages.Any())
                notifications.Add(NotificationMessage.Delete(dialogState.ChatMessages));
        }
        
        return CommandResult.Build(notifications.ToArray());
    }
}