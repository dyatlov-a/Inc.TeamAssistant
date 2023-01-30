using Inc.TeamAssistant.CheckIn.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

public sealed record GetLocationsQuery(MapId MapId) : IRequest<GetLocationsResult>;