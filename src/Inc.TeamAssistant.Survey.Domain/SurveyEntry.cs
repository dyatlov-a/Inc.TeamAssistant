namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyEntry
{
    public Guid Id { get; private set; }
    public Guid TemplateId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public SurveyState State { get; private set; }

    private SurveyEntry()
    {
    }

    public SurveyEntry(Guid id, Guid roomId, Guid templateId, DateTimeOffset created)
        : this()
    {
        Id = id;
        RoomId = roomId;
        TemplateId = templateId;
        Created = created;
        State = SurveyState.InProgress;
    }

    public SurveyEntry MoveToFinish()
    {
        State = SurveyState.Completed;
        
        return this;
    }
}