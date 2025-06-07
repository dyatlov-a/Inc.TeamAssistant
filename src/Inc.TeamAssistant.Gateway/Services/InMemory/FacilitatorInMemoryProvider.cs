using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class FacilitatorInMemoryProvider : IFacilitatorProvider
{
    private readonly ConcurrentDictionary<Guid, long> _state = new();
    
    public long? Get(Guid teamId) => _state.TryGetValue(teamId, out var userId) ? userId : null;

    public void Set(Guid teamId, long personId) => _state.AddOrUpdate(teamId, k => personId, (k, v) => personId);
}