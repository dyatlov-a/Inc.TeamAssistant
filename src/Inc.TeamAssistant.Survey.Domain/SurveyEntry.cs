namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyEntry
{
    public Guid Id { get; private set; }
    public Guid TemplateId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public SurveyState State { get; private set; }
    public IReadOnlyCollection<Guid> QuestionIds { get; private set; } = default!;

    private SurveyEntry()
    {
    }

    public SurveyEntry(Guid id, Guid roomId, DateTimeOffset created, SurveyTemplate template)
        : this()
    {
        ArgumentNullException.ThrowIfNull(template);
        
        Id = id;
        TemplateId = template.Id;
        RoomId = roomId;
        Created = created;
        State = SurveyState.InProgress;
        QuestionIds = template.QuestionIds.ToArray();
    }

    public SurveyEntry MoveToFinish()
    {
        State = SurveyState.Completed;
        
        return this;
    }
}