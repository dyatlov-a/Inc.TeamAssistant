using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives;
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
            throw new ApplicationException($"Team {command.TeamId} was not found.");

        var person = await _personRepository.Find(command.MessageContext.PersonId, token);
        if (person is null)
            throw new ApplicationException($"Person {command.MessageContext.PersonId} was not found.");

        team.AddTeammate(person);

        await _teamRepository.Upsert(team, token);

        var message = await _messageBuilder.Build(
            Messages.Connector_JoinToTeamSuccess,
            command.MessageContext.LanguageId,
            team.Name);
        return CommandResult.Build(NotificationMessage.Create(command.MessageContext.ChatId, message));
    }
}