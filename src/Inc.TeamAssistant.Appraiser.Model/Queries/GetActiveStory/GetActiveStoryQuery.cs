using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;

public sealed record GetActiveStoryQuery(Guid TeamId)
    : IRequest<GetActiveStoryResult>;