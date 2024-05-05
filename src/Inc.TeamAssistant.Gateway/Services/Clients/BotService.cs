using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
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

    public async Task<ServiceResult<GetBotResult?>> GetBotById(Guid botId, long ownerId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetBotQuery(botId, ownerId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotResult?>(ex.Message);
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
}