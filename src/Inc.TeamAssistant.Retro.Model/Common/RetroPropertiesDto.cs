using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroPropertiesDto(
    long? FacilitatorId,
    Guid TemplateId,
    TimeSpan TimerDuration,
    int VoteCount,
    string RetroType)
    : IWithEmpty<RetroPropertiesDto>
{
    public static RetroPropertiesDto Empty { get; } = new(
        FacilitatorId: null,
        TemplateId: Guid.Empty,
        TimerDuration: TimeSpan.Zero,
        VoteCount: 0,
        RetroType: string.Empty);
}