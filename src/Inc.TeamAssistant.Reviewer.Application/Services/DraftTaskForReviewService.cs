using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class DraftTaskForReviewService
{
    private readonly ReviewerOptions _reviewerOptions;
    private readonly IDraftTaskForReviewRepository _draftTaskForReviewRepository;

    public DraftTaskForReviewService(
        ReviewerOptions reviewerOptions,
        IDraftTaskForReviewRepository draftTaskForReviewRepository)
    {
        _reviewerOptions = reviewerOptions ?? throw new ArgumentNullException(nameof(reviewerOptions));
        _draftTaskForReviewRepository =
            draftTaskForReviewRepository ?? throw new ArgumentNullException(nameof(draftTaskForReviewRepository));
    }

    public bool HasDescriptionAndLinks(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(description));
        
        var descriptionParts = description.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var links = descriptionParts.Where(t => _reviewerOptions.LinksPrefix.Any(t.Contains)).ToArray();

        return links.Any() && descriptionParts.Length > links.Length;
    }

    public async Task<IReadOnlyCollection<NotificationMessage>> Delete(
        DraftTaskForReview draft,
        CancellationToken token)
    {
        var notifications = new List<NotificationMessage>();
        
        await _draftTaskForReviewRepository.Delete(draft.Id, token);
        notifications.Add(NotificationMessage.Delete(new ChatMessage(draft.ChatId, draft.MessageId)));
        if (draft.PreviewMessageId.HasValue)
            notifications.Add(NotificationMessage.Delete(new ChatMessage(draft.ChatId, draft.PreviewMessageId.Value)));

        return notifications;
    }
}