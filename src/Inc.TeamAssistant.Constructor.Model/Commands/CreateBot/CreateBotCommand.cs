using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;

public sealed record CreateBotCommand(
    string Name,
    string Token,
    Guid CalendarId,
    IReadOnlyCollection<Guid> FeatureIds,
    IReadOnlyDictionary<string, string> Properties,
    IReadOnlyCollection<string> SupportedLanguages,
    IReadOnlyCollection<BotDetails> BotDetails)
    : IRequest;