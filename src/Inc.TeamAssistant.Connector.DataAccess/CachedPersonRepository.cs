using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class CachedPersonRepository : IPersonRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPersonRepository _repository;
    private readonly TimeSpan _cacheTimeout;

    public CachedPersonRepository(IMemoryCache memoryCache, IPersonRepository repository, TimeSpan cacheTimeout)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheTimeout = cacheTimeout;
    }

    public async Task<Person?> Find(long personId, CancellationToken token)
    {
        return await _memoryCache.GetOrCreateAsync(GetKey(personId), async c =>
        {
            c.AbsoluteExpirationRelativeToNow = _cacheTimeout;

            return await _repository.Find(personId, token);
        });
    }

    public async Task<Person?> Find(string username, CancellationToken token)
    {
        return await _repository.Find(username, token);
    }

    public async Task<Teammate?> Find(TeammateKey key, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(key);
        
        return await _repository.Find(key, token);
    }

    public async Task<IReadOnlyCollection<Person>> GetTeammates(
        Guid teamId,
        DateTimeOffset now,
        bool? canFinalize,
        CancellationToken token)
    {
        return await _repository.GetTeammates(teamId, now, canFinalize, token);
    }

    public async Task Upsert(Person person, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(person);
        
        var key = GetKey(person.Id);

        if (_memoryCache.TryGetValue(key, out Person? cachedPerson) && cachedPerson == person)
            return;

        await _repository.Upsert(person, token);
        _memoryCache.Remove(key);
    }

    public async Task RemoveFromTeam(TeammateKey key, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(key);
        
        await _repository.RemoveFromTeam(key, token);
    }

    public async Task Upsert(Teammate teammate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(teammate);
        
        await _repository.Upsert(teammate, token);
    }

    public async Task<Guid?> FindBotId(long personId, CancellationToken token)
    {
        return await _repository.FindBotId(personId, token);
    }

    private string GetKey(long personId) => $"{nameof(CachedPersonRepository)}_{personId}";
}