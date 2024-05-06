using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBot;

public sealed record GetBotQuery(Guid Id, long CurrentUserId) : IRequest<GetBotResult?>;