using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Validators;

internal sealed class AddStoryCommandValidator : AbstractValidator<AddStoryCommand>
{
    public AddStoryCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.StoryType)
            .NotEmpty();
        
        RuleFor(e => e.Title)
            .NotEmpty()
            .Must(e => !e.StartsWith("/"))
            .WithMessage("'{PropertyName}' please enter text value.");
        
        RuleForEach(e => e.Links)
            .NotEmpty();
        
        RuleFor(e => e.Teammates)
            .NotEmpty();
    }
}