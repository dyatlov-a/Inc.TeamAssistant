using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroSessionRepository
{
    Task Create(RetroSession retro, CancellationToken token);
}