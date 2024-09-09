using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;

public sealed record GetBotsByCurrentUserQuery : IRequest<GetBotsByCurrentUserResult>;