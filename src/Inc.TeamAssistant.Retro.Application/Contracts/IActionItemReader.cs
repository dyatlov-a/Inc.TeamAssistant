using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IActionItemReader
{
    Task<IReadOnlyCollection<ActionItem>> Read(
        Guid roomId,
        ActionItemState state,
        Guid? lastItemId,
        int pageSize,
        CancellationToken token);
}