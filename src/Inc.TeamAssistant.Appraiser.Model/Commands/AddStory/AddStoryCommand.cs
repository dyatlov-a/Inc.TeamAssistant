using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;

public sealed record AddStoryCommand(
    MessageContext MessageContext,
    Guid TeamId,
    string StoryType,
    string Title,
    IReadOnlyCollection<string> Links,
    IReadOnlyCollection<Person> Teammates)
    : IEndDialogCommand;