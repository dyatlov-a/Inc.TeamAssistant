using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam;

internal sealed class LeaveFromTeamCommandHandler : IRequestHandler<LeaveFromTeamCommand, CommandResult>
{
    private readonly ITeamRepository _repository;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IEnumerable<ILeaveTeamHandler> _leaveTeamHandlers;

    public LeaveFromTeamCommandHandler(
        ITeamRepository repository,
        IMessageBuilder messageBuilder,
        IEnumerable<ILeaveTeamHandler> leaveTeamHandlers)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _leaveTeamHandlers = leaveTeamHandlers ?? throw new ArgumentNullException(nameof(leaveTeamHandlers));
    }
    
    public async Task<CommandResult> Handle(LeaveFromTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var personId = command.MessageContext.Person.Id;
        var team = await command.TeamId.Required(_repository.Find, token);
        
        await _repository.Upsert(team.RemoveTeammate(personId), token);

        var notifications = new List<NotificationMessage>();
        
        foreach (var leaveTeamHandler in _leaveTeamHandlers)
            notifications.AddRange(await leaveTeamHandler.Handle(command.MessageContext, team.Id, token));

        var leaveSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_LeaveTeamSuccess,
            command.MessageContext.LanguageId,
            command.MessageContext.Person.DisplayName,
            team.Name);
        notifications.Add(NotificationMessage.Create(command.MessageContext.ChatMessage.ChatId, leaveSuccessMessage));
        
        if (personId != team.Owner.Id)
            notifications.Add(NotificationMessage.Create(team.Owner.Id, leaveSuccessMessage));
        
        return CommandResult.Build(notifications.ToArray());
    }
}