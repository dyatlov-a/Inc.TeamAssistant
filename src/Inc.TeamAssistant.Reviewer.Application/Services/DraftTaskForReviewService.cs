using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class DraftTaskForReviewService
{
    private readonly IDraftTaskForReviewRepository _draftTaskForReviewRepository;
    private readonly ITeamAccessor _teamAccessor;

    public DraftTaskForReviewService(
        IDraftTaskForReviewRepository draftTaskForReviewRepository,
        ITeamAccessor teamAccessor)
    {
        _draftTaskForReviewRepository =
            draftTaskForReviewRepository ?? throw new ArgumentNullException(nameof(draftTaskForReviewRepository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<bool> HasTeammate(Guid teamId, long personId, CancellationToken token)
    {
        var teammates = await _teamAccessor.GetTeammates(teamId, DateTimeOffset.UtcNow, token);
        
        return teammates.Any(t => t.Id == personId);
    }

    public bool HasDescriptionAndLinks(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        
        var descriptionParts = description.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var links = descriptionParts.Where(t => GlobalSettings.LinksPrefix.Any(t.Contains)).ToArray();

        return links.Any() && descriptionParts.Length > links.Length;
    }

    public async Task<IReadOnlyCollection<NotificationMessage>> Delete(
        DraftTaskForReview draft,
        CancellationToken token)
    {
        var notifications = new List<NotificationMessage>();
        
        if (draft.PreviewMessageId.HasValue)
            notifications.Add(NotificationMessage.Delete(new ChatMessage(draft.ChatId, draft.PreviewMessageId.Value)));
        
        notifications.Add(NotificationMessage.Delete(new ChatMessage(draft.ChatId, draft.MessageId)));
        
        await _draftTaskForReviewRepository.Delete(draft.Id, token);

        return notifications;
    }
}