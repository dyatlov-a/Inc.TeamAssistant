using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;

public sealed record GetRoomHistoryQuery(Guid RoomId)
    : IRequest<GetRoomHistoryResult>;