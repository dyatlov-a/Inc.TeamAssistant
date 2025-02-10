using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

public sealed record GetMapsResult(IReadOnlyCollection<MapDto> Items)
    : IWithEmpty<GetMapsResult>
{
    public static GetMapsResult Empty { get; } = new(Array.Empty<MapDto>());
}