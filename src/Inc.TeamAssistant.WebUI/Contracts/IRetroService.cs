using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<GetRetroStateResult> GetRetroState(Guid teamId, CancellationToken token = default);
    
    Task<StartRetroResult> StartRetro(StartRetroCommand command, CancellationToken token = default);
    
    Task MoveToNextRetroState(MoveToNextRetroStateCommand command, CancellationToken token = default);
}