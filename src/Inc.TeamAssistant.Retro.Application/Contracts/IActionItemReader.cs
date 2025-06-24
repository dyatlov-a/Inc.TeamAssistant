using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IActionItemReader
{
    Task<IReadOnlyCollection<ActionItem>> Read(
        Guid roomId,
        ActionItemState state,
        int offset,
        int limit,
        CancellationToken token);
}