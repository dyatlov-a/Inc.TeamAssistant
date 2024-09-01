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
    
    public async Task<GetChatsResult> GetChatsByBot(Guid botId, CancellationToken token)
    {
        return await _mediator.Send(new GetChatsQuery(botId), token);
    }

    public async Task<GetHistoryResult> GetHistory(Guid botId, long chatId, int depth, CancellationToken token)
    {
        return await _mediator.Send(new GetHistoryQuery(botId, chatId, depth), token);
    }
}