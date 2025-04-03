using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.ChangeTeamProperty;

internal sealed class ChangeTeamPropertyCommandHandler : IRequestHandler<ChangeTeamPropertyCommand, CommandResult>
{
    private readonly ITeamRepository _repository;
    private readonly IMessageBuilder _messageBuilder;

    public ChangeTeamPropertyCommandHandler(ITeamRepository repository, IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(ChangeTeamPropertyCommand command, CancellationToken token)
    {
        var team = await command.TeamId.Required(_repository.Find, token);
        
        await _repository.Upsert(team.ChangeProperty(new PropertyKey(command.Name), command.Value), token);

        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            _messageBuilder.Build(
                Messages.Connector_ChangedPropertySuccess,
                command.MessageContext.LanguageId,
                team.Name));
        return CommandResult.Build(notification);
    }
}