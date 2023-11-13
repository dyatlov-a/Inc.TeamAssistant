using System.Text;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.DialogContinuations;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

internal sealed class DynamicCommandFactory : ICommandFactory
{
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly AddStoryToAssessmentSessionOptions _options;

    public DynamicCommandFactory(
        IDialogContinuation<ContinuationState> dialogContinuation,
        AddStoryToAssessmentSessionOptions options)
	{
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _options = options ?? throw new ArgumentNullException(nameof(options));
	}

	public IRequest<CommandResult>? TryCreate(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var continuation = _dialogContinuation.Find(context.UserId.Value);

        if (continuation is null)
            return null;
        
		return continuation.ContinuationState switch
		{
            ContinuationState.EnterTitle => new ActivateAssessmentCommand(
                context.ChatId,
                new(context.UserId.Value),
                context.UserName,
                context.Cmd),
            ContinuationState.EnterStory => CreateAddStoryCommand(context),
			_ => null
		};
	}

    private AddStoryToAssessmentSessionCommand CreateAddStoryCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var storyItems = context.Cmd.Split(' ');
        var storyTitleBuilder = new StringBuilder();
        var links = new List<string>();

        foreach (var storyItem in storyItems)
        {
            if (_options.LinksPrefix.Any(l => storyItem.StartsWith(l, StringComparison.InvariantCultureIgnoreCase)))
                links.Add(storyItem.ToLower().Trim());
            else
                storyTitleBuilder.Append($"{storyItem} ");
        }

        return new(context.LanguageId, context.UserId, context.UserName, storyTitleBuilder.ToString(), links);
    }
}