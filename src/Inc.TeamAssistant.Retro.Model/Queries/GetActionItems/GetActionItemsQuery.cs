using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;

public sealed record GetActionItemsQuery(Guid RoomId, int PageSize)
    : IRequest<GetActionItemsResult>;