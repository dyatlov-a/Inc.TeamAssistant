using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<CreateRetroItemResult> CreateRetroItem(CreateRetroItemCommand command, CancellationToken token = default);
    
    Task UpdateRetroItem(UpdateRetroItemCommand command, CancellationToken token = default);
}