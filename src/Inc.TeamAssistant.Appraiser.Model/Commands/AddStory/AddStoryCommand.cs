using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;

public sealed record AddStoryCommand(
    MessageContext MessageContext,
    Guid TeamId,
    string StoryType,
    string Title,
    IReadOnlyCollection<string> Links,
    IReadOnlyCollection<(long PersonId, string PersonDisplayName)> Teammates)
    : IEndDialogCommand;