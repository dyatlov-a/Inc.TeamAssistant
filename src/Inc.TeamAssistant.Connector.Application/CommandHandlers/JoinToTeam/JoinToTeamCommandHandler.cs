using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
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
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);

        var person = await _personRepository.Find(command.MessageContext.PersonId, token);
        if (person is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, command.MessageContext.PersonId);

        team.AddTeammate(person);

        await _teamRepository.Upsert(team, token);

        var joinToTeamSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_JoinToTeamSuccess,
            command.MessageContext.LanguageId,
            person.DisplayName,
            team.Name);
        
        var notifications = new List<NotificationMessage>
        {
            NotificationMessage.Create(command.MessageContext.ChatId, joinToTeamSuccessMessage)
        };

        if (command.MessageContext.PersonId != team.OwnerId)
            notifications.Add(NotificationMessage.Create(team.OwnerId, joinToTeamSuccessMessage));
        
        return CommandResult.Build(notifications.ToArray());
    }
}