using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroTemplateReader
{
    Task<IReadOnlyCollection<RetroColumn>> GetColumns(Guid templateId, CancellationToken token);
}