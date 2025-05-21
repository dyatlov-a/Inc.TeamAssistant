using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class ParticipantViewModel
{
    public Person Person { get; private set; }
    public int TotalVotes { get; private set; }

    public ParticipantViewModel(Person person, int totalVotes)
    {
        ArgumentNullException.ThrowIfNull(person);

        Person = person;
        TotalVotes = totalVotes;
    }

    public ParticipantViewModel ChangeTotalVotes(int value)
    {
        TotalVotes = value;

        return this;
    }
}