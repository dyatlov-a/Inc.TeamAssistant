using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBot;

internal sealed class GetBotQueryHandler : IRequestHandler<GetBotQuery, GetBotResult?>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentUserResolver _currentUserResolver;

    public GetBotQueryHandler(IBotRepository botRepository, ICurrentUserResolver currentUserResolver)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentUserResolver = currentUserResolver ?? throw new ArgumentNullException(nameof(currentUserResolver));
    }

    public async Task<GetBotResult?> Handle(GetBotQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var currentUserId = _currentUserResolver.GetUserId();
        var bot = await _botRepository.FindById(query.Id, token);
        if (bot is null)
            return null;
        
        if (bot.OwnerId != currentUserId)
            throw new ApplicationException($"User {currentUserId} has not access to bot {query.Id}.");

        return new GetBotResult(bot.Id, bot.Name, bot.Token, bot.FeatureIds, bot.Properties);
    }
}