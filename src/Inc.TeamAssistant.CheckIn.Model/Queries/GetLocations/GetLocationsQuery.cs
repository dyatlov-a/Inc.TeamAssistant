using MediatR;

namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

public sealed record GetLocationsQuery(Guid MapId)
    : IRequest<GetLocationsResult>;