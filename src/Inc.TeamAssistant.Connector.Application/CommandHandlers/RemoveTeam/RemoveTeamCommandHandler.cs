using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeam;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam;

internal sealed class RemoveTeamCommandHandler : IRequestHandler<RemoveTeamCommand, CommandResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IEnumerable<IRemoveTeamHandler> _removeTeamHandlers;
    private readonly IMessageBuilder _messageBuilder;

    public RemoveTeamCommandHandler(
        ITeamRepository teamRepository,
        IEnumerable<IRemoveTeamHandler> removeTeamHandlers,
        IMessageBuilder messageBuilder)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _removeTeamHandlers = removeTeamHandlers ?? throw new ArgumentNullException(nameof(removeTeamHandlers));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(RemoveTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);
        
        if (team.Owner.Id != command.MessageContext.Person.Id)
            throw new TeamAssistantUserException(Messages.Connector_HasNotRightsForRemoveTeam, team.Name);

        foreach (var removeTeamHandler in _removeTeamHandlers)
            await removeTeamHandler.Handle(command.MessageContext, command.TeamId, token);
        
        var removeTeamSuccessMessage = await _messageBuilder.Build(
            Messages.Connector_RemoveTeamSuccess,
            command.MessageContext.LanguageId,
            team.Name);
        
        await _teamRepository.Remove(command.TeamId, token);

        return CommandResult.Build(NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            removeTeamSuccessMessage));
    }
}