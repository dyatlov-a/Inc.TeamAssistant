using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class PersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IMemoryCache _memoryCache;

    public PersonService(IPersonRepository personRepository, IMemoryCache memoryCache)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }
    
    public async Task<Person> EnsurePerson(User user, CancellationToken token)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        
        var key = $"person_{user.Id}";
        var currentPerson = new Person(
            user.Id,
            user.FirstName,
            user.LanguageCode,
            user.Username);
        
        if (_memoryCache.TryGetValue(key, out Person? person) && person?.IsEquivalent(currentPerson) == true)
            return currentPerson;

        await _personRepository.Upsert(currentPerson, token);
        _memoryCache.Set(key, currentPerson, TimeSpan.FromHours(1));
        
        return currentPerson;
    }
}