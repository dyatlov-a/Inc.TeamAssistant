using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;

internal sealed class AddStoryCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    private readonly AddStoryToAssessmentSessionOptions _options;
    
    private readonly string _command = "/add";
    private readonly BotCommandStage _commandStage = BotCommandStage.EnterText;
    
    public int Priority => 3;

    public AddStoryCommandCreator(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        IDialogContinuation<BotCommandStage> dialogContinuation,
        AddStoryToAssessmentSessionOptions options)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public async Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        var dialogState = _dialogContinuation.Find(messageContext.PersonId);
        if (messageContext.Cmd.Equals(_command, StringComparison.InvariantCultureIgnoreCase) &&
            messageContext.CurrentCommandStage == _commandStage &&
            dialogState is not null)
        {
            if (messageContext.Text.StartsWith("/"))
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        "Command",
                        await _messageBuilder.Build(Messages.Appraiser_StoryTitleIsEmpty, messageContext.LanguageId))
                });
            
            var teamId = Guid.Parse(dialogState.Data[1]);
            var teammates = await _teamAccessor.GetTeammates(teamId, token);
            var storyItems = messageContext.Text.Split(' ');
            var storyTitleBuilder = new StringBuilder();
            var links = new List<string>();

            foreach (var storyItem in storyItems)
            {
                if (_options.LinksPrefix.Any(l => storyItem.StartsWith(l, StringComparison.InvariantCultureIgnoreCase)))
                    links.Add(storyItem.ToLower().Trim());
                else
                    storyTitleBuilder.Append($"{storyItem} ");
            }
            
            return new AddStoryCommand(messageContext, teamId, storyTitleBuilder.ToString(), links, teammates);
        }

        return null;
    }
}