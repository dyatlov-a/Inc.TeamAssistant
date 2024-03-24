using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class CachedPersonRepository : IPersonRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPersonRepository _personRepository;
    private readonly TimeSpan _cacheTimeout;

    public CachedPersonRepository(IMemoryCache memoryCache, IPersonRepository personRepository, TimeSpan cacheTimeout)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _cacheTimeout = cacheTimeout;
    }

    public async Task<Person?> Find(long personId, CancellationToken token)
    {
        return await _memoryCache.GetOrCreateAsync(GetKey(personId), async c =>
        {
            c.AbsoluteExpirationRelativeToNow = _cacheTimeout;

            return await _personRepository.Find(personId, token);
        });
    }

    public async Task<Person?> Find(string username, CancellationToken token)
    {
        return await _personRepository.Find(username, token);
    }

    public async Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, CancellationToken token)
    {
        return await _personRepository.GetTeammates(teamId, token);
    }

    public async Task Upsert(Person person, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(person);
        
        var key = GetKey(person.Id);

        if (_memoryCache.TryGetValue(key, out Person? cachedPerson) && cachedPerson == person)
            return;

        await _personRepository.Upsert(person, token);
        _memoryCache.Remove(key);
    }
    
    private string GetKey(long personId) => $"{nameof(CachedPersonRepository)}_{personId}";
}