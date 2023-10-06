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

    public MessageBuilderService(ITranslateProvider translateProvider)
    {
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<(string Text, IReadOnlyCollection<MessageEntity> Entities)> NewTaskForReviewBuild(
        LanguageId languageId,
        TaskForReview taskForReview)
    {
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (taskForReview is null)
            throw new ArgumentNullException(nameof(taskForReview));

        var reviewerLink = taskForReview.Reviewer.GetPersonLink();
        var messageText = await _translateProvider.Get(
            Messages.Reviewer_NewTaskForReview,
            languageId,
            taskForReview.Description,
            taskForReview.Owner.GetFullName(),
            reviewerLink);
        var entities = taskForReview.Reviewer.HasUsername()
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
                        Id = taskForReview.Reviewer.Id,
                        LanguageCode = taskForReview.Reviewer.LanguageId.Value,
                        FirstName = taskForReview.Reviewer.FirstName,
                        LastName = taskForReview.Reviewer.LastName,
                        Username = taskForReview.Reviewer.Username
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