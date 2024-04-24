using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam;

internal sealed class JoinToTeamCommandHandler : IRequestHandler<JoinToTeamCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMessageBuilder _messageBuilder;

    public JoinToTeamCommandHandler(
        ITeamRepository teamRepository,
        IPersonRepository personRepository,
        IMessageBuilder messageBuilder)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(JoinToTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            return CommandResult.Empty;

        var person = await _personRepository.Find(command.MessageContext.Person.Id, token);
        if (person is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, command.MessageContext.Person.Id);

        team.AddTeammate(person);

        await _teamRepository.Upsert(team, token);

        var joinToTeamSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_JoinToTeamSuccess,
            command.MessageContext.LanguageId,
            person.DisplayName,
            team.Name);
        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            joinToTeamSuccessMessage);

        var notifications = command.MessageContext.Person.Id != team.OwnerId
            ? new[]
            {
                NotificationMessage.Create(team.OwnerId, joinToTeamSuccessMessage),
                notification
            }
            : [notification];
        
        return CommandResult.Build(notifications);
    }
}