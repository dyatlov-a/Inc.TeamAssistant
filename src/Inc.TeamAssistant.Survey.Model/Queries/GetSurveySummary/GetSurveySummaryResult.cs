using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record GetSurveySummaryResult(IReadOnlyCollection<SurveyQuestionDto> Answers)
    : IWithEmpty<GetSurveySummaryResult>
{
    public static GetSurveySummaryResult Empty { get; } = new(Answers: []);
}