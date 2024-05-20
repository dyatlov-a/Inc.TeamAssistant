using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;

public sealed record GetBotUserNameQuery(string Token) : IRequest<GetBotUserNameResult>;