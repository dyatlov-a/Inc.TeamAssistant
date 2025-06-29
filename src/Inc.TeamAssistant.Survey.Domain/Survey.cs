namespace Inc.TeamAssistant.Survey.Domain;

public sealed class Survey
{
    public Guid Id { get; private set; }
    public Guid TemplateId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public SurveyState State { get; private set; }
    public IReadOnlyCollection<Guid> QuestionIds { get; private set; } = default!;

    private Survey()
    {
    }

    public Survey(Guid id, Guid roomId, DateTimeOffset created, SurveyTemplate template)
        : this()
    {
        ArgumentNullException.ThrowIfNull(template);
        
        Id = id;
        TemplateId = template.Id;
        RoomId = roomId;
        Created = created;
        State = SurveyState.InProgress;
        QuestionIds = template.QuestionIds;
    }
}