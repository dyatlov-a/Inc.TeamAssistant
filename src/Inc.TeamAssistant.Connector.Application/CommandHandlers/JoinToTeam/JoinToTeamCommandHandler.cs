using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam;

internal sealed class JoinToTeamCommandHandler : IRequestHandler<JoinToTeamCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;

    public JoinToTeamCommandHandler(
        ITeamRepository teamRepository,
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(JoinToTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            return CommandResult.Empty;

        var person = await _teamAccessor.EnsurePerson(command.MessageContext.Person.Id, token);

        await _teamRepository.Upsert(team.AddTeammate(person), token);

        var joinToTeamSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_JoinToTeamSuccess,
            command.MessageContext.LanguageId,
            person.DisplayName,
            team.Name);
        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            joinToTeamSuccessMessage);

        var notifications = command.MessageContext.Person.Id != team.Owner.Id
            ? new[]
            {
                NotificationMessage.Create(team.Owner.Id, joinToTeamSuccessMessage),
                notification
            }
            : [notification];
        
        return CommandResult.Build(notifications);
    }
}