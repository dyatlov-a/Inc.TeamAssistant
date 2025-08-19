using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record GetSurveyStateResult(
    Guid? SurveyId,
    bool Finished,
    long? FacilitatorId,
    IReadOnlyCollection<AnswerOnSurveyDto> Questions,
    IReadOnlyCollection<PersonStateTicket> Participants)
    : IWithEmpty<GetSurveyStateResult>
{
    public static GetSurveyStateResult Empty { get; } = new(
        SurveyId: null,
        Finished: false,
        FacilitatorId: null,
        Questions: [],
        Participants: []);
}