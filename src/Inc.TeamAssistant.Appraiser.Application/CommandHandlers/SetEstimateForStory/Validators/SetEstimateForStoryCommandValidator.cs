using FluentValidation;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Validators;

internal sealed class SetEstimateForStoryCommandValidator : AbstractValidator<SetEstimateForStoryCommand>
{
    public SetEstimateForStoryCommandValidator()
    {
        RuleFor(e => e.StoryId)
            .NotEmpty();
        
        RuleFor(e => e.Value)
            .GreaterThan(Estimation.None.Value);
    }
}