using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.JoinToRoom;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.JoinToRoom;

internal sealed class JoinToRoomCommandHandler : IRequestHandler<JoinToRoomCommand>
{
    private readonly IOnlinePersonStore _store;
    private readonly IPersonResolver _personResolver;
    private readonly IRoomEventSender _eventSender; 

    public JoinToRoomCommandHandler(
        IOnlinePersonStore store,
        IPersonResolver personResolver,
        IRoomEventSender eventSender)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(JoinToRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var persons = _store.JoinToTeam(command.RoomId, command.ConnectionId, currentPerson);
        
        await _eventSender.PersonsChanged(command.RoomId, persons);
    }
}