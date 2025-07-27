using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

public sealed record GetRoomPropertiesResult(
    long? FacilitatorId,
    Guid RetroTemplateId,
    Guid SurveyTemplateId,
    TimeSpan TimerDuration,
    int VoteCount,
    int VoteByItemCount,
    string RetroType,
    IReadOnlyCollection<TemplateDto> RetroTemplates,
    IReadOnlyCollection<TemplateDto> SurveyTemplates)
    : IWithEmpty<GetRoomPropertiesResult>
{
    public static GetRoomPropertiesResult Empty { get; } = new(
        FacilitatorId: null,
        RetroTemplateId: Guid.Empty,
        SurveyTemplateId: Guid.Empty,
        TimerDuration: TimeSpan.Zero,
        VoteCount: 0,
        VoteByItemCount: 0,
        RetroType: string.Empty,
        RetroTemplates: [],
        SurveyTemplates: []);
}