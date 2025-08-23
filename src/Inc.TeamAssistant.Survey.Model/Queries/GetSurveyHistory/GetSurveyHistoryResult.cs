using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Survey.Model.Common;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyHistory;

public sealed record GetSurveyHistoryResult(IReadOnlyCollection<SurveyQuestionDto> Answers)
    : IWithEmpty<GetSurveyHistoryResult>
{
    public static GetSurveyHistoryResult Empty { get; } = new(Answers: []);
}