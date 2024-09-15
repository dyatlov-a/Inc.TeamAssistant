using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IBotService
{
    Task<GetBotUserNameResult> Check(GetBotUserNameQuery query, CancellationToken token = default);

    Task<GetBotResult> GetBotById(Guid botId, CancellationToken token = default);
    
    Task<GetBotsByCurrentUserResult> GetFromCurrentUser(CancellationToken token = default);
    
    Task<GetWidgetsResult> GetWidgetsForCurrentUser(Guid botId, CancellationToken token = default);
    
    Task<GetTeammatesResult> GetTeammates(Guid teamId, CancellationToken token = default);

    Task<GetTeamConnectorResult> GetConnector(
        Guid teamId,
        string foreground,
        string background,
        CancellationToken token = default);

    Task RemoveTeammate(RemoveTeammateCommand command, CancellationToken token = default);
    
    Task<GetFeaturesResult> GetFeatures(CancellationToken token = default);
    
    Task<GetPropertiesResult> GetProperties(CancellationToken token = default);

    Task<GetBotDetailsResult> GetDetails(GetBotDetailsQuery query, CancellationToken token = default);

    Task Create(CreateBotCommand command, CancellationToken token = default);

    Task Update(UpdateBotCommand command, CancellationToken token = default);
    
    Task Remove(Guid botId, CancellationToken token = default);
    
    Task SetDetails(SetBotDetailsCommand command, CancellationToken token = default);
}