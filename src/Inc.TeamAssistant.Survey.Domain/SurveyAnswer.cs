namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyAnswer
{
    public Guid Id { get; private set; }
    public Guid SurveyId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long OwnerId { get; private set; }
    public IReadOnlyCollection<Answer> Answers { get; private set; } = [];

    private SurveyAnswer()
    {
    }

    public SurveyAnswer(
        Guid id,
        Guid surveyId,
        DateTimeOffset created,
        long ownerId)
        : this()
    {
        Id = id;
        SurveyId = surveyId;
        Created = created;
        OwnerId = ownerId;
    }

    public SurveyAnswer SetAnswer(Answer answer)
    {
        ArgumentNullException.ThrowIfNull(answer);

        Answers = Answers
            .Where(a => a.QuestionId != answer.QuestionId)
            .Append(answer)
            .ToArray();
        
        return this;
    }
}