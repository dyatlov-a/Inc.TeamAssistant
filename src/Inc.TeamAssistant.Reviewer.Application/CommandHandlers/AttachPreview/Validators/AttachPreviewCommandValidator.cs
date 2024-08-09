using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachPreview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AttachPreview.Validators;

internal sealed class AttachPreviewCommandValidator : AbstractValidator<AttachPreviewCommand>
{
    public AttachPreviewCommandValidator()
    {
        RuleFor(e => e.DraftId)
            .NotEmpty();
        
        RuleFor(e => e.MessageId)
            .NotEmpty();
    }
}