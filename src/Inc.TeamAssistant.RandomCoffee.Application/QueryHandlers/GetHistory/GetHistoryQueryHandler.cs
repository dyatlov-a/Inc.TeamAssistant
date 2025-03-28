using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Application.QueryHandlers.GetHistory.Converters;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.QueryHandlers.GetHistory;

internal sealed class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, GetHistoryResult>
{
    private readonly IRandomCoffeeReader _reader;
    private readonly RandomCoffeeHistoryConverter _converter;

    public GetHistoryQueryHandler(IRandomCoffeeReader reader, RandomCoffeeHistoryConverter converter)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public async Task<GetHistoryResult> Handle(GetHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var historyItems = await _reader.GetHistory(query.BotId, query.ChatId, query.Depth, token);
        var results = await _converter.ConvertTo(historyItems, token);
        
        return new GetHistoryResult(results);
    }
}