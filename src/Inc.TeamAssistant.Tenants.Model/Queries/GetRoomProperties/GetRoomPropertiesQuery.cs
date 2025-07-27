using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

public sealed record GetRoomPropertiesQuery(Guid RoomId)
    : IRequest<GetRoomPropertiesResult>;