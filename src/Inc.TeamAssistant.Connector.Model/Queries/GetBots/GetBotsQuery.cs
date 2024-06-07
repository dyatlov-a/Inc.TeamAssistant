using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetBots;

public sealed record GetBotsQuery(long UserId) : IRequest<GetBotsResult>;