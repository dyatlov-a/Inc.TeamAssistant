using System.Collections.Immutable;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record GetRetroStateResult(
    RetroSessionDto? ActiveSession,
    IReadOnlyCollection<RetroItemDto> Items,
    IReadOnlyDictionary<long, int> TotalVotes,
    IReadOnlyCollection<Person> OnlinePersons)
    : IWithEmpty<GetRetroStateResult>
{
    public static GetRetroStateResult Empty { get; } = new(
        ActiveSession: null,
        Items: [],
        TotalVotes: ImmutableDictionary<long, int>.Empty,
        OnlinePersons: []);
}