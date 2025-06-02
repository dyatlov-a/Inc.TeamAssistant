using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IActionItemReader
{
    Task<IReadOnlyCollection<ActionItem>> Read(Guid teamId, CancellationToken token);
}