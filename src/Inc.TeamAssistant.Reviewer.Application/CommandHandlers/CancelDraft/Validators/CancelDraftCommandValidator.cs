using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft.Validators;

internal sealed class CancelDraftCommandValidator : AbstractValidator<CancelDraftCommand>
{
    public CancelDraftCommandValidator()
    {
        RuleFor(e => e.DraftId)
            .NotEmpty();
    }
}