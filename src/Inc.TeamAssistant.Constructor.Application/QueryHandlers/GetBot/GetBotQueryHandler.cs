using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBot;

internal sealed class GetBotQueryHandler : IRequestHandler<GetBotQuery, GetBotResult?>
{
    private readonly IBotRepository _botRepository;

    public GetBotQueryHandler(IBotRepository botRepository)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task<GetBotResult?> Handle(GetBotQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var bot = await _botRepository.FindById(query.Id, token);
        if (bot is null)
            return null;
        
        if (bot.OwnerId != query.OwnerId)
            throw new ApplicationException($"User {query.OwnerId} has not access to bot {query.Id}.");

        return new GetBotResult(bot.Id, bot.Name, bot.Token, bot.FeatureIds);
    }
}