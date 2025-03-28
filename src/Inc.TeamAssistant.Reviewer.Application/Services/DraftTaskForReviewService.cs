using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class DraftTaskForReviewService
{
    private readonly IDraftTaskForReviewRepository _repository;
    private readonly ITeamAccessor _teamAccessor;

    public DraftTaskForReviewService(IDraftTaskForReviewRepository repository, ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
        ArgumentNullException.ThrowIfNull(draft);
        
        var notifications = new List<NotificationMessage>();
        
        if (draft.PreviewMessageId.HasValue)
            notifications.Add(NotificationMessage.Delete(new ChatMessage(draft.ChatId, draft.PreviewMessageId.Value)));
        
        notifications.Add(NotificationMessage.Delete(new ChatMessage(draft.ChatId, draft.MessageId)));
        
        await _repository.Delete(draft.Id, token);

        return notifications;
    }
}