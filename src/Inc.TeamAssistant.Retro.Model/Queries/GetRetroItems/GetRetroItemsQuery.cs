using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

public sealed record GetRetroItemsQuery(Guid TeamId)
    : IRequest<GetRetroItemsResult>;