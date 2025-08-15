namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record QuestionAnswerDto(
    DateTimeOffset Created,
    long PersonId,
    int Value);