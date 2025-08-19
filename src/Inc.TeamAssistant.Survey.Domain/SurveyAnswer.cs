namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyAnswer
{
    public Guid SurveyId { get; private set; }
    public Guid QuestionId { get; private set; }
    public long ResponderId { get; private set; }
    public DateTimeOffset Responded { get; private set; }
    public int Value { get; private set; }
    public string? Comment { get; private set; }

    private SurveyAnswer()
    {
    }

    public SurveyAnswer(
        Guid surveyId,
        Guid questionId,
        long responderId,
        DateTimeOffset responded,
        int value,
        string? comment)
        : this()
    {
        SurveyId = surveyId;
        QuestionId = questionId;
        ResponderId = responderId;
        Responded = responded;
        Value = value;
        Comment = comment;
    }
}