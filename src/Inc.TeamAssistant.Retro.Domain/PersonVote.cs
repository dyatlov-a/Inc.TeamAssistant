namespace Inc.TeamAssistant.Retro.Domain;

public sealed class PersonVote
{
    public Guid RetroSessionId { get; private set; }
    public long PersonId { get; private set; }
    public IReadOnlyCollection<PersonVoteByItem> Votes { get; private set; }

    private PersonVote()
    {
        Votes = [];
    }

    public PersonVote(Guid retroSessionId, long personId, IReadOnlyCollection<PersonVoteByItem> votes)
        : this()
    {
        RetroSessionId = retroSessionId;
        PersonId = personId;
        Votes = votes ?? throw new ArgumentNullException(nameof(votes));
    }
}