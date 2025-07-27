using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record GetSurveyStateResult(
    long? FacilitatorId,
    IReadOnlyCollection<SurveyQuestionDto> Questions)
    : IWithEmpty<GetSurveyStateResult>
{
    public static GetSurveyStateResult Empty { get; } = new(FacilitatorId: null, Questions: []);
}