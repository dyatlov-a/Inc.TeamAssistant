using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;

public sealed record GetRetroTemplatesResult(IReadOnlyCollection<RetroTemplateDto> Templates)
    : IWithEmpty<GetRetroTemplatesResult>
{
    public static GetRetroTemplatesResult Empty { get; } = new([]);
}