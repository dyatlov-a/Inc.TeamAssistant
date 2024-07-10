using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;

public sealed record GetPropertiesQuery
    : IRequest<GetPropertiesResult>;