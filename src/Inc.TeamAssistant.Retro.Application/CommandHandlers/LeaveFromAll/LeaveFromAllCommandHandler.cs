using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.LeaveFromAll;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.LeaveFromAll;

internal sealed class LeaveFromAllCommandHandler : IRequestHandler<LeaveFromAllCommand, LeaveFromAllResult>
{
    private readonly IOnlinePersonStore _store;
    private readonly IRetroEventSender _eventSender; 

    public LeaveFromAllCommandHandler(IOnlinePersonStore store, IRetroEventSender eventSender)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }
    
    public async Task<LeaveFromAllResult> Handle(LeaveFromAllCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var teamIds = new List<Guid>();
        var teams = _store.LeaveFromAllTeams(
            command.ConnectionId,
            (tId, p, t) => _eventSender.PersonsChanged(tId, p),
            token);
        
        await foreach (var teamId in teams)
            teamIds.Add(teamId);

        return new(teamIds);
    }
}