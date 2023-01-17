using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed record DbPlayer
{
    public Guid Id { get; init; }
    public Guid TeamId { get; init; }
    public long PersonId { get; init; }
    public LanguageId PersonLanguageId { get; init; } = default!;
    public string PersonFirstName { get; init; } = default!;
    public string? PersonLastName { get; init; }
    public string? PersonUsername { get; init; }
}