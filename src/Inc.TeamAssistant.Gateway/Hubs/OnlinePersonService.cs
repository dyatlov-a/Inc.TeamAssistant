using System.Collections.Concurrent;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class OnlinePersonService
{
    private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, Person>> _store = new();
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _retroEvent;

    public OnlinePersonService(IPersonResolver personResolver, IRetroEventSender retroEvent)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _retroEvent = retroEvent ?? throw new ArgumentNullException(nameof(retroEvent));
    }

    public async Task AddToTeam(string connectionId, Guid teamId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var persons = _store.GetOrAdd(teamId, _ => new ConcurrentDictionary<string, Person>());
        
        persons.TryAdd(connectionId, currentPerson);

        await BroadcastPersons(teamId);
    }

    public async Task LeaveFromTeam(string connectionId, Guid teamId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        
        if (_store.TryGetValue(teamId, out var persons))
            persons.TryRemove(connectionId, out _);
        
        await BroadcastPersons(teamId);
    }

    public async Task<IReadOnlyCollection<Guid>> LeaveFromAllTeams(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        var teams = new List<Guid>();
        
        foreach (var (teamId, value) in _store)
            if (value.TryRemove(connectionId, out _))
            {
                await BroadcastPersons(teamId);
                
                teams.Add(teamId);
            }

        return teams;
    }

    private async Task BroadcastPersons(Guid teamId)
    {
        if (_store.TryGetValue(teamId, out var persons))
            await _retroEvent.PersonsChanged(teamId, persons.Values.ToArray());
    }
}