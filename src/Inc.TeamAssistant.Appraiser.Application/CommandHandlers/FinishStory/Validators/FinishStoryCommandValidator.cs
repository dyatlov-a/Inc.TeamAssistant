using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishStory;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishStory.Validators;

internal sealed class FinishStoryCommandValidator : AbstractValidator<FinishStoryCommand>
{
    public FinishStoryCommandValidator()
    {
        RuleFor(e => e.StoryId)
            .NotEmpty();
    }
}