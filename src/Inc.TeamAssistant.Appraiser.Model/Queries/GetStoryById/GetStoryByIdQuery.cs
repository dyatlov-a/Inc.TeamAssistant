using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryById;

public sealed record GetStoryByIdQuery(Guid Id)
    : IRequest<GetStoryByIdResult?>;