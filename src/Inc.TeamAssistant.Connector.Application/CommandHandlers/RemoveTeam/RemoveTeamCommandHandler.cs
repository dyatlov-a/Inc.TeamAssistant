using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeam;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam;

internal sealed class RemoveTeamCommandHandler : IRequestHandler<RemoveTeamCommand, CommandResult>
{
    private readonly ITeamRepository _repository;
    private readonly IEnumerable<IRemoveTeamHandler> _removeTeamHandlers;
    private readonly IMessageBuilder _messageBuilder;

    public RemoveTeamCommandHandler(
        ITeamRepository repository,
        IEnumerable<IRemoveTeamHandler> removeTeamHandlers,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _removeTeamHandlers = removeTeamHandlers ?? throw new ArgumentNullException(nameof(removeTeamHandlers));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(RemoveTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await command.TeamId.Required(_repository.Find, token);
        
        if (team.Owner.Id != command.MessageContext.Person.Id)
            throw new TeamAssistantUserException(Messages.Connector_HasNotRightsForRemoveTeam, team.Name);

        foreach (var removeTeamHandler in _removeTeamHandlers)
            await removeTeamHandler.Handle(command.MessageContext, command.TeamId, token);
        
        await _repository.Remove(command.TeamId, token);

        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            _messageBuilder.Build(
                Messages.Connector_RemoveTeamSuccess,
                command.MessageContext.LanguageId,
                team.Name));
        return CommandResult.Build(notification);
    }
}