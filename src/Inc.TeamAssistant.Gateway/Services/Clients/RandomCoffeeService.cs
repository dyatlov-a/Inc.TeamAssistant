using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class RandomCoffeeService : IRandomCoffeeService
{
    private readonly IMediator _mediator;

    public RandomCoffeeService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<ServiceResult<GetChatsResult>> GetChatsByBot(Guid botId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetChatsQuery(botId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetChatsResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetHistoryResult>> GetHistory(
        Guid botId,
        long chatId,
        int depth,
        CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetHistoryQuery(botId, chatId, depth), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetHistoryResult>(ex.Message);
        }
    }
}