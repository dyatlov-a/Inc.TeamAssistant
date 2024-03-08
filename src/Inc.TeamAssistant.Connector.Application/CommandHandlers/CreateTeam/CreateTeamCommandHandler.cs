using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam;

internal sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMessageBuilder _messageBuilder;
    private readonly ILinkBuilder _linkBuilder;

    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        IMessageBuilder messageBuilder,
        ILinkBuilder linkBuilder)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
    }

    public async Task<CommandResult> Handle(CreateTeamCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var team = new Team(
            command.MessageContext.BotId,
            command.MessageContext.ChatId,
            command.Name,
            command.Properties);

        await _teamRepository.Upsert(team, token);
        
        var message = await _messageBuilder.Build(
            Messages.Connector_JoinToTeam,
            command.MessageContext.LanguageId,
            team.Name,
            _linkBuilder.BuildLinkForConnect(command.BotName, team.Id));
        var notifications = new List<NotificationMessage>
        {
            NotificationMessage.Create(command.MessageContext.ChatId, message, pinned: true)
        };
        
        return CommandResult.Build(notifications.ToArray());
    }
}