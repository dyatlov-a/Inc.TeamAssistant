using FluentValidation;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Validators;

internal sealed class MoveToReviewCommandValidator : AbstractValidator<MoveToReviewCommand>
{
    private readonly IDraftTaskForReviewRepository _draftTaskForReviewRepository;
    private readonly DraftTaskForReviewService _draftTaskForReviewService;
    
    public MoveToReviewCommandValidator(
        IDraftTaskForReviewRepository draftTaskForReviewRepository,
        DraftTaskForReviewService draftTaskForReviewService)
    {
        _draftTaskForReviewRepository =
            draftTaskForReviewRepository ?? throw new ArgumentNullException(nameof(draftTaskForReviewRepository));
        _draftTaskForReviewService =
            draftTaskForReviewService ?? throw new ArgumentNullException(nameof(draftTaskForReviewService));

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
        
        var draft = await _draftTaskForReviewRepository.GetById(draftId, token);

        if (!_draftTaskForReviewService.HasDescriptionAndLinks(draft.Description))
            context.AddFailure(
                nameof(draft.Description),
                "Must contains a link to the source code and some description");
        
        if (draft.TargetPersonId.HasValue &&
            !await _draftTaskForReviewService.HasTeammate(draft.TeamId, draft.TargetPersonId.Value, token))
            context.AddFailure(nameof(draft.TargetPersonId), "Teammate not found");
    }
}