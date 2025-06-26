using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroPropertiesDto(
    long? FacilitatorId,
    Guid RetroTemplateId,
    Guid SurveyTemplateId,
    TimeSpan TimerDuration,
    int VoteCount,
    int VoteByItemCount,
    string RetroType)
    : IWithEmpty<RetroPropertiesDto>
{
    public static RetroPropertiesDto Empty { get; } = new(
        FacilitatorId: null,
        RetroTemplateId: Guid.Empty,
        SurveyTemplateId: Guid.Empty,
        TimerDuration: TimeSpan.Zero,
        VoteCount: 0,
        VoteByItemCount: 0,
        RetroType: string.Empty);
}