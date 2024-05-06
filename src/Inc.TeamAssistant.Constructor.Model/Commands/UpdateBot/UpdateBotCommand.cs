using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;

public sealed record UpdateBotCommand(
    Guid Id,
    string Name,
    string Token,
    long CurrentUserId,
    IReadOnlyCollection<Guid> FeatureIds,
    IReadOnlyDictionary<string, string> Properties)
    : IRequest;