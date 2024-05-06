using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;

public sealed record CreateBotCommand(
    string Name,
    string Token,
    long OwnerId,
    IReadOnlyCollection<Guid> FeatureIds,
    IReadOnlyDictionary<string, string> Properties)
    : IRequest;