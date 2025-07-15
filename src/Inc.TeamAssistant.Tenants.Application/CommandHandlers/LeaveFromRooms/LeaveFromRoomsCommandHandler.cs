using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRooms;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.LeaveFromRooms;

internal sealed class LeaveFromRoomsCommandHandler : IRequestHandler<LeaveFromRoomsCommand, LeaveFromRoomsResult>
{
    private readonly IOnlinePersonStore _store;
    private readonly IRoomEventSender _eventSender; 

    public LeaveFromRoomsCommandHandler(IOnlinePersonStore store, IRoomEventSender eventSender)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }
    
    public async Task<LeaveFromRoomsResult> Handle(LeaveFromRoomsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var teamIds = new List<Guid>();
        var teams = _store.LeaveFromTeams(
            command.ConnectionId,
            (tId, p, t) => _eventSender.PersonsChanged(tId, p),
            token);
        
        await foreach (var teamId in teams)
            teamIds.Add(teamId);

        return new(teamIds);
    }
}