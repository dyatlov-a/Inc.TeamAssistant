using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;

public sealed record SetBotDetailsCommand(
    string Token,
    IReadOnlyCollection<BotDetails> BotDetails)
    : IRequest;