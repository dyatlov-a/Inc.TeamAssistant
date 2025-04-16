using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam;

internal sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, CommandResult>
{
    private readonly ITeamRepository _repository;
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamLinkBuilder _teamLinkBuilder;

    public CreateTeamCommandHandler(
        ITeamRepository repository,
        IMessageBuilder messageBuilder,
        ITeamLinkBuilder teamLinkBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamLinkBuilder = teamLinkBuilder ?? throw new ArgumentNullException(nameof(teamLinkBuilder));
    }

    public async Task<CommandResult> Handle(CreateTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = new Team(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.Person,
            command.Name,
            command.MessageContext.Bot.Properties);

        await _repository.Upsert(team, token);

        var linkForConnect = _teamLinkBuilder.BuildLinkForConnect(command.BotName, team.Id);
        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            _messageBuilder.Build(
                Messages.Connector_JoinToTeam,
                command.MessageContext.LanguageId,
                team.Name,
                linkForConnect),
            pinned: true);
        
        return CommandResult.Build(notification);
    }
}