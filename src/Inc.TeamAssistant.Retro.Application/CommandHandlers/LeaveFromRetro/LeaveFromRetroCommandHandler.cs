using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.LeaveFromRetro;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.LeaveFromRetro;

internal sealed class LeaveFromRetroCommandHandler : IRequestHandler<LeaveFromRetroCommand>
{
    private readonly IOnlinePersonStore _store;
    private readonly IRetroEventSender _eventSender; 

    public LeaveFromRetroCommandHandler(IOnlinePersonStore store, IRetroEventSender eventSender)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }
    
    public async Task Handle(LeaveFromRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var persons = _store.LeaveFromTeam(command.RoomId, command.ConnectionId);
        
        await _eventSender.PersonsChanged(command.RoomId, persons);
    }
}