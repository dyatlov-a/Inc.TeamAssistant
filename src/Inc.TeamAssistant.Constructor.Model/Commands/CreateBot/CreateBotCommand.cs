using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;

public sealed record CreateBotCommand(
    string Name,
    string Token,
    IReadOnlyCollection<Guid> FeatureIds,
    IReadOnlyDictionary<string, string> Properties,
    IReadOnlyCollection<string> SupportedLanguages)
    : IRequest;