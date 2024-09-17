using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;
using Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;
using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class BotService : IBotService
{
    private readonly IMediator _mediator;

    public BotService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetBotUserNameResult> Check(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return await _mediator.Send(query, token);
    }

    public async Task<GetBotResult> GetBotById(Guid botId, CancellationToken token)
    {
        return await _mediator.Send(new GetBotQuery(botId), token);
    }

    public async Task<GetBotsByCurrentUserResult> GetFromCurrentUser(CancellationToken token)
    {
        return await _mediator.Send(new GetBotsByCurrentUserQuery(), token);
    }

    public async Task<GetWidgetsResult> GetWidgetsForCurrentUser(Guid botId, CancellationToken token)
    {
        return await _mediator.Send(new GetWidgetsQuery(botId), token);
    }

    public async Task UpdateWidgets(UpdateWidgetsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }

    public async Task<GetTeammatesResult> GetTeammates(Guid teamId, CancellationToken token)
    {
        return await _mediator.Send(new GetTeammatesQuery(teamId), token);
    }

    public async Task<GetTeamConnectorResult> GetConnector(
        Guid teamId,
        string foreground,
        string background,
        CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);
        
        return await _mediator.Send(new GetTeamConnectorQuery(teamId, foreground, background), token);
    }

    public async Task RemoveTeammate(RemoveTeammateCommand command, CancellationToken token)
    {
        await _mediator.Send(command, token);
    }

    public async Task<GetFeaturesResult> GetFeatures(CancellationToken token)
    {
        return await _mediator.Send(new GetFeaturesQuery(), token);
    }

    public async Task<GetPropertiesResult> GetProperties(CancellationToken token)
    {
        return await _mediator.Send(new GetPropertiesQuery(), token);
    }

    public async Task<GetBotDetailsResult> GetDetails(GetBotDetailsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return await _mediator.Send(query, token);
    }

    public async Task Create(CreateBotCommand command, CancellationToken token)
    {
        await _mediator.Send(command, token);
    }

    public async Task Update(UpdateBotCommand command, CancellationToken token)
    {
        await _mediator.Send(command, token);
    }

    public async Task Remove(Guid botId, CancellationToken token)
    {
        await _mediator.Send(new RemoveBotCommand(botId), token);
    }

    public async Task SetDetails(SetBotDetailsCommand command, CancellationToken token)
    {
        await _mediator.Send(command, token);
    }
}