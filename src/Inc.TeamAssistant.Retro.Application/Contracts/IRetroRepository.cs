using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroRepository
{
    Task Upsert(RetroItem item, CancellationToken token);
}