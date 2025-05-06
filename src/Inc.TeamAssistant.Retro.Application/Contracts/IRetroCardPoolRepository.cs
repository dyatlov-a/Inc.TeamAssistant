using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroCardPoolRepository
{
    Task<RetroCardPool?> Find(Guid id, CancellationToken token);

    Task Upsert(RetroCardPool pool, CancellationToken token);
}