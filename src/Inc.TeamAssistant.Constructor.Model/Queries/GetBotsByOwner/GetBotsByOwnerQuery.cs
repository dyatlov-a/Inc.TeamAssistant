using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;

public sealed record GetBotsByOwnerQuery(long OwnerId) : IRequest<GetBotsByOwnerResult>;