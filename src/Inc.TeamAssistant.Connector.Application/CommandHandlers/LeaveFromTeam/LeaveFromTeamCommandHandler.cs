using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
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
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);

        team.RemoveTeammate(command.MessageContext.PersonId);

        await _teamRepository.Upsert(team, token);

        foreach (var leaveTeamHandler in _leaveTeamHandlers)
            await leaveTeamHandler.Handle(command.MessageContext, command.TeamId, token);

        var leaveTeamSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_LeaveTeamSuccess,
            command.MessageContext.LanguageId,
            command.MessageContext.DisplayUsername,
            team.Name);
        
        var notifications = new List<NotificationMessage>
        {
            NotificationMessage.Create(command.MessageContext.ChatId, leaveTeamSuccessMessage)
        };
        
        if (command.MessageContext.PersonId != team.OwnerId)
            notifications.Add(NotificationMessage.Create(team.OwnerId, leaveTeamSuccessMessage));
        
        return CommandResult.Build(notifications.ToArray());
    }
}