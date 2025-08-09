using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;

namespace Inc.TeamAssistant.WebUI.Features.RoomProperties;

public sealed class ParticipantViewModel
{
    public Person Person { get; private set; }
    public bool IsOnline { get; private set; }
    public int TotalVotes { get; private set; }
    public bool Finished { get; private set; }
    public bool HandRaised { get; private set; }

    public ParticipantViewModel(
        Person person,
        bool isOnline,
        int totalVotes,
        bool finished,
        bool handRaised)
    {
        ArgumentNullException.ThrowIfNull(person);

        Person = person;
        IsOnline = isOnline;
        TotalVotes = totalVotes;
        Finished = finished;
        HandRaised = handRaised;
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
    
    public ParticipantViewModel ChangeHandRaised(bool value)
    {
        HandRaised = value;

        return this;
    }

    public SetRetroStateCommand ToCommand(Guid roomId)
    {
        return new SetRetroStateCommand(roomId, Person.Id, Finished, HandRaised);
    }
}