using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;

public sealed record GetRoomQuery(Guid Id)
    : IRequest<GetRoomResult>;