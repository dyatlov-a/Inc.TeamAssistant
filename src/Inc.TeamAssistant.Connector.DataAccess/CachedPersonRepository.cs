using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Hybrid;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class CachedPersonRepository : IPersonRepository
{
    private readonly HybridCache _cache;
    private readonly IPersonRepository _repository;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public CachedPersonRepository(HybridCache cache, IPersonRepository repository, TimeSpan cacheTimeout)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = cacheTimeout
        };
    }

    public async Task<Person?> Find(long personId, CancellationToken token)
    {
        return await _cache.GetOrCreateAsync(
            GetKey(personId),
            personId,
            async (pId, t) => await _repository.Find(pId, t),
            _cacheOptions,
            cancellationToken: token);
    }

    public async Task<Person?> Find(string username, CancellationToken token)
    {
        return await _cache.GetOrCreateAsync(
            GetKey(username),
            username,
            async (u, t) => await _repository.Find(u, t),
            _cacheOptions,
            cancellationToken: token);
    }

    public async Task Upsert(Person person, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(person);

        var cachedPerson = await Find(person.Id, token);
        
        if (cachedPerson == person)
            return;

        await _repository.Upsert(person, token);
        
        await _cache.RemoveAsync(GetKey(person.Id), token);
        
        if (!string.IsNullOrWhiteSpace(person.Username))
            await _cache.RemoveAsync(GetKey(person.Username), token);
    }

    private string GetKey(long personId) => $"{nameof(CachedPersonRepository)}_id_{personId}";
    
    private string GetKey(string username) => $"{nameof(CachedPersonRepository)}_username_{username}";
}