using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

public sealed record CreateRetroItemResult(Guid Id)
    : IWithEmpty<CreateRetroItemResult>
{
    public static CreateRetroItemResult Empty { get; } = new(Guid.Empty);
}