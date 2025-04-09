using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
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
    private readonly ITeamAccessor _teamAccessor;
    private readonly IEnumerable<ILeaveTeamHandler> _leaveTeamHandlers;

    public LeaveFromTeamCommandHandler(
        ITeamRepository repository,
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        IEnumerable<ILeaveTeamHandler> leaveTeamHandlers)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _leaveTeamHandlers = leaveTeamHandlers ?? throw new ArgumentNullException(nameof(leaveTeamHandlers));
    }
    
    public async Task<CommandResult> Handle(LeaveFromTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var personId = command.MessageContext.Person.Id;
        var team = await command.TeamId.Required(_repository.Find, token);
        
        await _repository.Upsert(team.RemoveTeammate(personId), token);
        
        var leaveMessageForPerson = BuildLeaveFromTeamMessage(command.MessageContext.LanguageId);
        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            leaveMessageForPerson);
        var notifications = new List<NotificationMessage> { notification };

        if (personId != team.Owner.Id)
        {
            var ownerLanguageId = await _teamAccessor.GetClientLanguage(
                command.MessageContext.Bot.Id,
                team.Owner.Id,
                token);
            var leaveMessageForTeamOwner = BuildLeaveFromTeamMessage(ownerLanguageId);
            
            notifications.Add(NotificationMessage.Create(team.Owner.Id, leaveMessageForTeamOwner));
        }
        
        foreach (var leaveTeamHandler in _leaveTeamHandlers)
            notifications.AddRange(await leaveTeamHandler.Handle(command.MessageContext, team.Id, token));
        
        return CommandResult.Build(notifications.ToArray());

        string BuildLeaveFromTeamMessage(LanguageId languageId)
        {
            return _messageBuilder.Build(
                Messages.Connector_LeaveTeamSuccess,
                languageId,
                command.MessageContext.Person.DisplayName,
                team.Name);
        }
    }
}