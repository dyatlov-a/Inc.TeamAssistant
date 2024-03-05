using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

public sealed record GetStoryDetailsQuery(Guid TeamId)
    : IRequest<GetStoryDetailsResult?>;