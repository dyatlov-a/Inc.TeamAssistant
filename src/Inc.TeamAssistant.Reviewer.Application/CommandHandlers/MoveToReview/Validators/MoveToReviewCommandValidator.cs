using FluentValidation;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Validators;

internal sealed class MoveToReviewCommandValidator : AbstractValidator<MoveToReviewCommand>
{
    private readonly IDraftTaskForReviewRepository _repository;
    private readonly DraftTaskForReviewService _service;
    
    public MoveToReviewCommandValidator(IDraftTaskForReviewRepository repository, DraftTaskForReviewService service)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));

        RuleFor(e => e.DraftId)
            .NotEmpty()
            .CustomAsync(ValidateDraft);
    }

    private async Task ValidateDraft(
        Guid draftId,
        ValidationContext<MoveToReviewCommand> context,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        var draft = await _repository.GetById(draftId, token);

        if (!_service.HasDescriptionAndLinks(draft.Description))
            context.AddFailure(
                nameof(draft.Description),
                "Must contains a link to the source code and some description");
        
        if (draft.TargetPersonId.HasValue &&
            !await _service.HasTeammate(draft.TeamId, draft.TargetPersonId.Value, token))
            context.AddFailure(nameof(draft.TargetPersonId), "Teammate not found");
    }
}