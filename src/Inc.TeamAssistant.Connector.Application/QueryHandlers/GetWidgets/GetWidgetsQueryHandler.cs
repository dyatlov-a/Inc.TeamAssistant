using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets;

internal sealed class GetWidgetsQueryHandler : IRequestHandler<GetWidgetsQuery, GetWidgetsResult>
{
    private readonly IBotReader _botReader;
    private readonly ICurrentPersonResolver _currentPersonResolver;

    public GetWidgetsQueryHandler(IBotReader botReader, ICurrentPersonResolver currentPersonResolver)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
    }

    public async Task<GetWidgetsResult> Handle(GetWidgetsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var person = _currentPersonResolver.GetCurrentPerson();
        var bots = await _botReader.GetBotsByUser(person.Id, token);
        var bot = bots.SingleOrDefault(b => b.Id == query.BotId);
        
        var hasReviewer = bot?.Features.Contains("Reviewer", StringComparer.InvariantCultureIgnoreCase) == true;
        var hasAppraiser = bot?.Features.Contains("Appraiser", StringComparer.InvariantCultureIgnoreCase) == true;
        var hasRandomCoffee = bot?.Features.Contains("RandomCoffee", StringComparer.InvariantCultureIgnoreCase) == true;
        var hasCheckIn = bot?.Features.Contains("CheckIn", StringComparer.InvariantCultureIgnoreCase) == true;

        return new GetWidgetsResult([
            new("TeammatesWidget", 1, true, true),
            new("ReviewTotalStatsWidget", 2, hasReviewer, hasReviewer),
            new("ReviewHistoryWidget", 3, hasReviewer, hasReviewer),
            new("ReviewAverageStatsWidget", 4, hasReviewer, hasReviewer),
            new("AppraiserHistoryWidget", 5, hasAppraiser, hasAppraiser),
            new("AppraiserIntegrationWidget", 6, hasAppraiser, hasAppraiser),
            new("RandomCoffeeHistoryWidget", 7, hasRandomCoffee, hasRandomCoffee),
            new("MapWidget", 8, hasCheckIn, hasCheckIn)
        ]);
    }
}