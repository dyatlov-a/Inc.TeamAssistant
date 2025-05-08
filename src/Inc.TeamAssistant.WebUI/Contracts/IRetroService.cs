using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<CreateRetroItemResult> CreateRetroItem(CreateRetroItemCommand command, CancellationToken token = default);
}