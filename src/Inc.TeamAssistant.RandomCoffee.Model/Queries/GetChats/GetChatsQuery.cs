using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;

public sealed record GetChatsQuery(Guid BotId)
    : IRequest<GetChatsResult>;