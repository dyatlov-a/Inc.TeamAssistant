using Inc.TeamAssistant.Connector.Application.Contracts;
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

    public async Task LeaveFromTeam(Guid teamId, long personId, CancellationToken token)
    {
        await _repository.LeaveFromTeam(teamId, personId, token);
    }

    public async Task LeaveFromTeam(Guid teamId, long personId, DateTimeOffset? until, CancellationToken token)
    {
        await _repository.LeaveFromTeam(teamId, personId, until, token);
    }

    public async Task<Guid?> FindBotId(long personId, CancellationToken token)
    {
        return await _repository.FindBotId(personId, token);
    }

    private string GetKey(long personId) => $"{nameof(CachedPersonRepository)}_{personId}";
}