using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;

public sealed record GetAvailableRoomsQuery(Guid? RoomId)
    : IRequest<GetAvailableRoomsResult>;