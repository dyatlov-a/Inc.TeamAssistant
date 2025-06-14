using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record RetroPropertiesDto(
    long? FacilitatorId,
    Guid TemplateId,
    TimeSpan TimerDuration,
    int VoteCount)
    : IWithEmpty<RetroPropertiesDto>
{
    public static RetroPropertiesDto Empty { get; } = new(
        FacilitatorId: null,
        TemplateId: Guid.Empty,
        TimerDuration: TimeSpan.Zero,
        VoteCount: 0);
}