using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableTeams;

public sealed record GetAvailableTeamsQuery(Guid? TeamId)
    : IRequest<GetAvailableTeamsResult>;