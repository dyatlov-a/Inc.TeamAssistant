using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;

internal sealed class SetEstimateForStoryCommandCreator : ICommandCreator
{
    public string Command { get; }

    public SetEstimateForStoryCommandCreator(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));
        
        Command = command;
    }
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(
            Command,
            string.Empty,
            StringComparison.InvariantCultureIgnoreCase));
                
        return Task.FromResult<IRequest<CommandResult>>(new SetEstimateForStoryCommand(
            messageContext,
            storyId,
            Command));
    }
}