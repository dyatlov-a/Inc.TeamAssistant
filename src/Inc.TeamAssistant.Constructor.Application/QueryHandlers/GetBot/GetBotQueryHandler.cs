using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBot;

internal sealed class GetBotQueryHandler : IRequestHandler<GetBotQuery, GetBotResult>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;

    public GetBotQueryHandler(IBotRepository botRepository, ICurrentPersonResolver currentPersonResolver)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentPersonResolver = currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
    }

    public async Task<GetBotResult> Handle(GetBotQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        var bot = await _botRepository.FindById(query.Id, token);
        if (bot is null)
            throw new ApplicationException($"Bot {query.Id} was not found.");
        
        if (bot.OwnerId != currentPerson.Id)
            throw new ApplicationException($"User {currentPerson.Id} has not access to bot {query.Id}.");

        return new GetBotResult(
            bot.Id,
            bot.CalendarId,
            bot.Name,
            bot.Token,
            bot.FeatureIds,
            bot.Properties,
            bot.SupportedLanguages);
    }
}