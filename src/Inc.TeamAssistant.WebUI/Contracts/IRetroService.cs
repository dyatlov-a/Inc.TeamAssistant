using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItemsHistory;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroHistory;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<GetRetroStateResult> GetRetroState(Guid roomId, CancellationToken token = default);
    
    Task<GetRetroHistoryResult> GetRetroHistory(Guid sessionId, CancellationToken token = default);
    
    Task StartRetro(StartRetroCommand command, CancellationToken token = default);
    
    Task MoveToNextRetroState(MoveToNextRetroStateCommand command, CancellationToken token = default);

    Task<GetActionItemsResult> GetActionItems(Guid roomId, int limit, CancellationToken token = default);
    
    Task<GetActionItemsHistoryResult> GetActionItemsHistory(
        Guid roomId,
        string state,
        int offset,
        int limit,
        CancellationToken token = default);
    
    Task ChangeActionItem(ChangeActionItemCommand command, CancellationToken token = default);
    
    Task<GetRetroAssessmentResult> GetRetroAssessment(Guid sessionId, CancellationToken token = default);
    
    Task SetRetroAssessment(SetRetroAssessmentCommand command, CancellationToken token = default);
}