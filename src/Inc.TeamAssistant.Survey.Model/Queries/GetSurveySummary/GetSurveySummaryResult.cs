using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record GetSurveySummaryResult(
    IReadOnlyCollection<PersonAnswerDto> Answers,
    IReadOnlyCollection<SurveyQuestionDto> AnswerByQuestions,
    int TotalAnswers)
    : IWithEmpty<GetSurveySummaryResult>
{
    public static GetSurveySummaryResult Empty { get; } = new(Answers: [], AnswerByQuestions: [], TotalAnswers: 0);
}