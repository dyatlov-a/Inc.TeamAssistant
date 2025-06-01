using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IActionItemRepository
{
    Task<ActionItem?> Find(Guid id, CancellationToken token);

    Task Upsert(ActionItem item, CancellationToken token);
}