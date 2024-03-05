using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.ChangeTeamProperty;

internal sealed class ChangeTeamPropertyCommandHandler : IRequestHandler<ChangeTeamPropertyCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMessageBuilder _messageBuilder;

    public ChangeTeamPropertyCommandHandler(ITeamRepository teamRepository, IMessageBuilder messageBuilder)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(ChangeTeamPropertyCommand command, CancellationToken token)
    {
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);

        team.ChangeProperty(command.Name, command.Value);
        await _teamRepository.Upsert(team, token);
        
        var message = await _messageBuilder.Build(
            Messages.Connector_ChangedPropertySuccess,
            command.MessageContext.LanguageId,
            team.Name);
        return CommandResult.Build(NotificationMessage.Create(command.MessageContext.ChatId, message));
    }
}