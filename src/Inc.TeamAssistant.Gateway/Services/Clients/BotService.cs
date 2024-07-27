using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
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

    public async Task<ServiceResult<GetBotsByOwnerResult>> GetBotsByOwner(long ownerId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetBotsByOwnerQuery(ownerId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotsByOwnerResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetBotUserNameResult>> Check(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        try
        {
            var result = await _mediator.Send(query, token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotUserNameResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetBotResult?>> GetBotById(Guid botId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetBotQuery(botId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotResult?>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetBotsResult>> GetByUser(long userId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetBotsQuery(userId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotsResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetTeammatesResult>> GetTeammates(Guid teamId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetTeammatesQuery(teamId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetTeammatesResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetTeamConnectorResult>> GetConnector(
        Guid teamId,
        string foreground,
        string background,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(foreground))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(foreground));
        if (string.IsNullOrWhiteSpace(background))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(background));
        
        try
        {
            var result = await _mediator.Send(new GetTeamConnectorQuery(teamId, foreground, background), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetTeamConnectorResult>(ex.Message);
        }
    }

    public async Task RemoveTeammate(RemoveTeammateCommand command, CancellationToken token)
    {
        try
        {
            await _mediator.Send(command, token);
        }
        catch
        {
            // ignored
        }
    }

    public async Task<ServiceResult<GetFeaturesResult>> GetFeatures(CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetFeaturesQuery(), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetFeaturesResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetPropertiesResult>> GetProperties(CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetPropertiesQuery(), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetPropertiesResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetBotDetailsResult>> GetDetails(GetBotDetailsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        try
        {
            var result = await _mediator.Send(query, token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotDetailsResult>(ex.Message);
        }
    }

    public async Task Create(CreateBotCommand command, CancellationToken token)
    {
        try
        {
            await _mediator.Send(command, token);
        }
        catch
        {
            // ignored
        }
    }

    public async Task Update(UpdateBotCommand command, CancellationToken token)
    {
        try
        {
            await _mediator.Send(command, token);
        }
        catch
        {
            // ignored
        }
    }

    public async Task Remove(Guid botId, CancellationToken token)
    {
        try
        {
            await _mediator.Send(new RemoveBotCommand(botId), token);
        }
        catch
        {
            // ignored
        }
    }
}