using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRoom;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.LeaveFromRoom;

internal sealed class LeaveFromRoomCommandHandler : IRequestHandler<LeaveFromRoomCommand>
{
    private readonly IOnlinePersonStore _store;
    private readonly IRoomEventSender _eventSender; 

    public LeaveFromRoomCommandHandler(IOnlinePersonStore store, IRoomEventSender eventSender)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }
    
    public async Task Handle(LeaveFromRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var persons = _store.LeaveFromTeam(command.RoomId, command.ConnectionId);
        
        await _eventSender.PersonsChanged(command.RoomId, persons);
    }
}