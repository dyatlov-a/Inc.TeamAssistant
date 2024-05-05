using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBot;

public sealed record GetBotQuery(Guid Id, long OwnerId) : IRequest<GetBotResult?>;