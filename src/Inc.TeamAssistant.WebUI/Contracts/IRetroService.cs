using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<GetRetroItemsResult> GetItems(Guid teamId, CancellationToken token = default);
    
    Task<CreateRetroItemResult> CreateRetroItem(CreateRetroItemCommand command, CancellationToken token = default);
    
    Task UpdateRetroItem(UpdateRetroItemCommand command, CancellationToken token = default);
    
    Task RemoveRetroItem(Guid retroItemId, CancellationToken token = default);
    
    Task<StartRetroResult> StartRetro(StartRetroCommand command, CancellationToken token = default);
}