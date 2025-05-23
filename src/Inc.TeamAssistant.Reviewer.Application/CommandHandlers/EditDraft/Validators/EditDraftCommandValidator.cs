using FluentValidation;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Model.Commands.EditDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.EditDraft.Validators;

internal sealed class EditDraftCommandValidator : AbstractValidator<EditDraftCommand>
{
    public EditDraftCommandValidator()
    {
        RuleFor(e => e.Description)
            .NotEmpty()
            .MaximumLength(2000)
            .Must(e => !e.HasCommand())
            .WithMessage("'{PropertyName}' please enter text value");
    }
}