using System.Text;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class MessageBuilderService : IMessageBuilderService
{
    private readonly ITranslateProvider _translateProvider;
    private readonly ITeamAccessor _teamAccessor;

    public MessageBuilderService(ITranslateProvider translateProvider, ITeamAccessor teamAccessor)
    {
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<(string Text, IReadOnlyCollection<MessageEntity> Entities)> NewTaskForReviewBuild(
        LanguageId languageId,
        TaskForReview taskForReview,
        CancellationToken token)
    {
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (taskForReview is null)
            throw new ArgumentNullException(nameof(taskForReview));

        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (!reviewer.HasValue)
            throw new ApplicationException($"Reviewer {taskForReview.ReviewerId} was not found.");
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (!owner.HasValue)
            throw new ApplicationException($"Owner {taskForReview.OwnerId} was not found.");

        var hasUsername = !string.IsNullOrWhiteSpace(reviewer.Value.Username);
        var reviewerLink = hasUsername
            ? reviewer.Value.Username!
            : reviewer.Value.Name;
        var messageText = await _translateProvider.Get(
            Messages.Reviewer_NewTaskForReview,
            languageId,
            taskForReview.Description,
            owner.Value.Name,
            reviewerLink);
        var entities = hasUsername
            ? Array.Empty<MessageEntity>()
            : new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.TextMention,
                    Offset = messageText.IndexOf(reviewerLink, StringComparison.InvariantCultureIgnoreCase),
                    Length = reviewerLink.Length,
                    User = new User
                    {
                        Id = taskForReview.ReviewerId,
                        LanguageCode = reviewer.Value.LanguageId.Value,
                        FirstName = reviewer.Value.Name,
                        Username = reviewer.Value.Username
                    }
                }
            };
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(messageText);
        var state = taskForReview.State switch
        {
            TaskForReviewState.New => "⏳",
            TaskForReviewState.InProgress => "🤩",
            TaskForReviewState.OnCorrection => "😱",
            TaskForReviewState.IsArchived => "👍",
            _ => throw new ArgumentOutOfRangeException($"State {taskForReview.State} out of range for {nameof(TaskForReviewState)}.")
        };
        messageBuilder.AppendLine(state);

        return (messageBuilder.ToString(), entities);
    }
}