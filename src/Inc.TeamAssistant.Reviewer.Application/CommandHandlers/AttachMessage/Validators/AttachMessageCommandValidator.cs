using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AttachMessage.Validators;

internal sealed class AttachMessageCommandValidator : AbstractValidator<AttachMessageCommand>
{
    public AttachMessageCommandValidator()
    {
        RuleFor(e => e.TaskId)
            .NotEmpty();
        
        RuleFor(e => e.MessageId)
            .NotEmpty();
    }
}