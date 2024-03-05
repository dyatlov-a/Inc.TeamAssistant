using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AttachStory.Validators;

internal sealed class AttachStoryCommandValidator : AbstractValidator<AttachStoryCommand>
{
    public AttachStoryCommandValidator()
    {
        RuleFor(e => e.StoryId)
            .NotEmpty();
        
        RuleFor(e => e.MessageId)
            .NotEmpty();
    }
}