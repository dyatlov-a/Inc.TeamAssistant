namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyAnswer
{
    public Guid Id { get; private set; }
    public Guid SurveyId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long OwnerId { get; private set; }
    public IReadOnlyCollection<Answer> Answers { get; private set; } = default!;

    private SurveyAnswer()
    {
    }

    public SurveyAnswer(
        Guid id,
        Guid surveyId,
        DateTimeOffset created,
        long ownerId,
        IReadOnlyCollection<Answer> answers)
        : this()
    {
        Id = id;
        SurveyId = surveyId;
        Created = created;
        OwnerId = ownerId;
        Answers = answers ?? throw new ArgumentNullException(nameof(answers));
    }
}