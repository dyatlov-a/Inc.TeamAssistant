using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroCardPool;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroCardPool;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<CreateRetroCardPoolResult> CreateRetroCardPool(
        CreateRetroCardPoolCommand command,
        CancellationToken token = default);
    
    Task UpdateRetroCardPool(
        UpdateRetroCardPoolCommand command,
        CancellationToken token = default);
}