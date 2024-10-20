using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;

public sealed record UpdateBotCommand(
    Guid Id,
    string Name,
    string Token,
    Guid CalendarId,
    IReadOnlyCollection<Guid> FeatureIds,
    IReadOnlyDictionary<string, string> Properties,
    IReadOnlyCollection<string> SupportedLanguages)
    : IRequest;