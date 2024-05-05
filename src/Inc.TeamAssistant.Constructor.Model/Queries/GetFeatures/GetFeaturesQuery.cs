using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;

public sealed record GetFeaturesQuery : IRequest<GetFeaturesResult>;