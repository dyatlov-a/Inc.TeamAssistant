using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record GetRetroStateResult(
    RetroSessionDto? ActiveSession,
    IReadOnlyCollection<RetroItemDto> Items,
    IReadOnlyCollection<RetroParticipantDto> Participants,
    IReadOnlyCollection<ActionItemDto> ActionItems,
    IReadOnlyCollection<RetroColumnDto> Columns,
    RetroPropertiesDto RetroProperties,
    TimeSpan? CurrentTimer)
    : IWithEmpty<GetRetroStateResult>
{
    public static GetRetroStateResult Empty { get; } = new(
        ActiveSession: null,
        Items: [],
        Participants: [],
        ActionItems: [],
        Columns: [],
        RetroPropertiesDto.Empty,
        CurrentTimer: null);
}