using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record GetSurveyStateResult(
    Guid? SurveyId,
    long? FacilitatorId,
    IReadOnlyCollection<SurveyQuestionDto> Questions)
    : IWithEmpty<GetSurveyStateResult>
{
    public static GetSurveyStateResult Empty { get; } = new(SurveyId: null, FacilitatorId: null, Questions: []);
}