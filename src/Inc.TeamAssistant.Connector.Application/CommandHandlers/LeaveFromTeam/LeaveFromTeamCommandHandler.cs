using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam;

internal sealed class LeaveFromTeamCommandHandler : IRequestHandler<LeaveFromTeamCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IEnumerable<ILeaveTeamHandler> _leaveTeamHandlers;

    public LeaveFromTeamCommandHandler(
        ITeamRepository teamRepository,
        IMessageBuilder messageBuilder,
        IEnumerable<ILeaveTeamHandler> leaveTeamHandlers)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _leaveTeamHandlers = leaveTeamHandlers ?? throw new ArgumentNullException(nameof(leaveTeamHandlers));
    }
    
    public async Task<CommandResult> Handle(LeaveFromTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);

        team.RemoveTeammate(command.MessageContext.Person.Id);

        var notifications = new List<NotificationMessage>();
        
        foreach (var leaveTeamHandler in _leaveTeamHandlers)
            notifications.AddRange(await leaveTeamHandler.Handle(command.MessageContext, team.Id, token));
        
        await _teamRepository.Upsert(team, token);

        var leaveTeamSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_LeaveTeamSuccess,
            command.MessageContext.LanguageId,
            command.MessageContext.Person.DisplayName,
            team.Name);
        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            leaveTeamSuccessMessage);
        notifications.Add(notification);
        
        if (command.MessageContext.Person.Id != team.OwnerId)
            notifications.Add(NotificationMessage.Create(team.OwnerId, leaveTeamSuccessMessage));
        
        return CommandResult.Build(notifications.ToArray());
    }
}