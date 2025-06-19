using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record GetRetroStateQuery(Guid RoomId)
    : IRequest<GetRetroStateResult>;