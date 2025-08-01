using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class PersonResolver : IPersonResolver
{
    private Person? _currentPerson;

    public void TrySet(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        
        _currentPerson ??= person;
    }

    public void Reset() => _currentPerson = null;

    public Person GetCurrentPerson()
    {
        if (_currentPerson is null)
            throw new InvalidOperationException("Current person is not set. Use TrySetCurrentPerson method to set it.");
        
        return _currentPerson;
    }
}