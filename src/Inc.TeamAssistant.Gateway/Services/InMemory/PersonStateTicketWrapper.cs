using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class PersonStateTicketWrapper
{
    private readonly Person _person;
    private readonly List<string> _connectionIds = new();
    
    private int _totalVote;
    private bool _finished;
    private bool _handRaised;
    
    public DateTimeOffset LastConnection { get; private set; }
    public IReadOnlyCollection<string> ConnectionIds => _connectionIds.AsReadOnly();

    public PersonStateTicketWrapper(Person person)
    {
        _person = person ?? throw new ArgumentNullException(nameof(person));
    }

    public PersonStateTicketWrapper ChangeTotalVote(int value)
    {
        _totalVote = value;

        return this;
    }
    
    public PersonStateTicketWrapper ChangeFinished(bool value)
    {
        _finished = value;

        return this;
    }
    
    public PersonStateTicketWrapper ChangeHandRaised(bool value)
    {
        _handRaised = value;

        return this;
    }

    public void Clear()
    {
        _totalVote = 0;
        _finished = false;
        _handRaised = false;
    }

    public PersonStateTicketWrapper Connected(string connectionId, DateTimeOffset now)
    {
        if (_connectionIds.All(c => c != connectionId))
            _connectionIds.Add(connectionId);

        LastConnection = now;

        return this;
    }
    
    public bool Disconnected(string connectionId) => _connectionIds.Remove(connectionId);

    public PersonStateTicket ToPersonStateTicket() => new(
        _person,
        IsOnline: _connectionIds.Any(),
        _totalVote,
        _finished,
        _handRaised);
}