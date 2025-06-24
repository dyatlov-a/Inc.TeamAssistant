using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetActionItemsHistory;

public sealed record GetActionItemsHistoryQuery(
    Guid RoomId,
    string State,
    int Offset,
    int Limit) 
    : IRequest<GetActionItemsHistoryResult>;