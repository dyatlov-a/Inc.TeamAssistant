using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;

internal sealed class SetEstimateForStoryCommandCreator : ICommandCreator
{
    private static readonly IReadOnlyCollection<string> Commands = AssessmentValue.GetAssessments
        .Select(a => a.ToString())
        .ToArray();
    
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        foreach (var command in Commands)
        {
            if (messageContext.Cmd.StartsWith($"/{command}", StringComparison.InvariantCultureIgnoreCase))
            {
                var storyId = Guid.Parse(messageContext.Cmd.Replace(
                    $"/{command}?storyId=",
                    string.Empty,
                    StringComparison.InvariantCultureIgnoreCase));
                
                return Task.FromResult<IRequest<CommandResult>?>(new SetEstimateForStoryCommand(
                    messageContext,
                    storyId,
                    command));
            }
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}