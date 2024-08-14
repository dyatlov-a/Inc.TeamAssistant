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
            .MustAsync(HasDescriptionAndLinks)
            .WithMessage("'Description' must contains a link to the source code and some description");
    }

    private async Task<bool> HasDescriptionAndLinks(Guid draftId, CancellationToken token)
    {
        var draft = await _draftTaskForReviewRepository.GetById(draftId, token);

        return _draftTaskForReviewService.HasDescriptionAndLinks(draft.Description);
    }
}