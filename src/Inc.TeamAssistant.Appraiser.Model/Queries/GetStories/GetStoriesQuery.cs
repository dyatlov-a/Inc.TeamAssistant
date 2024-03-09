using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

public sealed record GetStoriesQuery(Guid TeamId, DateOnly AssessmentDate)
    : IRequest<GetStoriesResult>;