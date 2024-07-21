using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;

public sealed record GetBotDetailsQuery(string Token)
    : IRequest<GetBotDetailsResult>;