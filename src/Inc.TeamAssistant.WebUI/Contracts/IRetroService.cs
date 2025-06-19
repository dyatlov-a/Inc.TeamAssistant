using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<GetRetroStateResult> GetRetroState(Guid teamId, CancellationToken token = default);
    
    Task StartRetro(StartRetroCommand command, CancellationToken token = default);
    
    Task MoveToNextRetroState(MoveToNextRetroStateCommand command, CancellationToken token = default);

    Task<GetActionItemsResult> GetActionItems(Guid teamId, CancellationToken token = default);
    
    Task ChangeActionItem(ChangeActionItemCommand command, CancellationToken token = default);
    
    Task<GetRetroAssessmentResult> GetRetroAssessment(Guid sessionId, CancellationToken token = default);
    
    Task SetRetroAssessment(SetRetroAssessmentCommand command, CancellationToken token = default);

    Task<GetRetroTemplatesResult> GetRetroTemplates(CancellationToken token = default);

    Task ChangeRetroProperties(ChangeRetroPropertiesCommand command, CancellationToken token = default);
}