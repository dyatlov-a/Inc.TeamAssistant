using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class ParticipantViewModel
{
    public Person Person { get; private set; }
    public int TotalVotes { get; private set; }
    public bool Finished { get; private set; }

    public ParticipantViewModel(Person person, int totalVotes, bool finished)
    {
        ArgumentNullException.ThrowIfNull(person);

        Person = person;
        TotalVotes = totalVotes;
        Finished = finished;
    }

    public ParticipantViewModel ChangeTotalVotes(int value)
    {
        TotalVotes = value;

        return this;
    }
    
    public ParticipantViewModel ChangeFinished(bool value)
    {
        Finished = value;

        return this;
    }
}