using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

public sealed record GetHistoryQuery(Guid BotId, long ChatId, int Depth)
    : IRequest<GetHistoryResult>;