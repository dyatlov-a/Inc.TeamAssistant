using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record GetRetroStateResult(
    RetroSessionDto? ActiveSession,
    IReadOnlyCollection<RetroItemDto> Items,
    IReadOnlyCollection<ParticipantDto> Participants,
    IReadOnlyCollection<ActionItemDto> ActionItems,
    TimeSpan? CurrentTimer)
    : IWithEmpty<GetRetroStateResult>
{
    public static GetRetroStateResult Empty { get; } = new(
        ActiveSession: null,
        Items: [],
        Participants: [],
        ActionItems: [],
        CurrentTimer: null);
}