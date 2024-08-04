using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft;

internal sealed class CancelDraftCommandHandler : IRequestHandler<CancelDraftCommand, CommandResult>
{
    private readonly IDraftTaskForReviewRepository _draftTaskForReviewRepository;
    private readonly DraftTaskForReviewService _draftTaskForReviewService;

    public CancelDraftCommandHandler(
        IDraftTaskForReviewRepository draftTaskForReviewRepository,
        DraftTaskForReviewService draftTaskForReviewService)
    {
        _draftTaskForReviewRepository =
            draftTaskForReviewRepository ?? throw new ArgumentNullException(nameof(draftTaskForReviewRepository));
        _draftTaskForReviewService =
            draftTaskForReviewService ?? throw new ArgumentNullException(nameof(draftTaskForReviewService));
    }

    public async Task<CommandResult> Handle(CancelDraftCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(nameof(command));

        var draft = await _draftTaskForReviewRepository.GetById(command.DraftId, token);

        var notifications = await _draftTaskForReviewService.Delete(draft, token);

        return CommandResult.Build(notifications.ToArray());
    }
}