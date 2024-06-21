using MediatR;

namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

public sealed record GetMapsQuery(Guid BotId)
    : IRequest<GetMapsResult>;