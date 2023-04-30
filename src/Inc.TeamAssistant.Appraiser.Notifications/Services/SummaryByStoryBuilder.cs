using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Notifications.Services;

internal sealed class SummaryByStoryBuilder
{
	private readonly IMessageBuilder _messageBuilder;

    public SummaryByStoryBuilder(IMessageBuilder messageBuilder)
	{
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

	public async Task<NotificationMessage> Build(LanguageId languageId, SummaryByStory summary, bool estimateEnded)
    {
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
		if (summary is null)
			throw new ArgumentNullException(nameof(summary));

		var builder = new StringBuilder();

        await AddStoryDetails(
            builder,
            estimateEnded ? Messages.EndEstimate : Messages.NeedEstimate,
            languageId,
            summary.Story);
        AddEstimates(builder, summary.Items, estimateEnded);

        if (estimateEnded)
            await AddTotalEstimate(builder, languageId, summary);
        
        var messageText = builder.ToString();
        
        var message = estimateEnded
            ? NotificationMessage.Create(summary.ChatId, messageText)
            : AddAssessments(NotificationMessage.Edit(
                new [] { (summary.ChatId, summary.Story.ExternalId) },
                messageText));

        return message;
    }

	public NotificationMessage AddAssessments(NotificationMessage notificationMessage)
	{
		if (notificationMessage is null)
			throw new ArgumentNullException(nameof(notificationMessage));

        foreach (var assessment in AssessmentValue.GetAssessments)
        {
            var value = assessment.ToString();
            
            notificationMessage.WithButton(
                new Button(value.Replace("sp", string.Empty, StringComparison.InvariantCultureIgnoreCase),
                    value));
        }
        
        return notificationMessage;
    }

    public async Task AddStoryDetails(
        StringBuilder stringBuilder,
        MessageId titleTemplate,
        LanguageId languageId,
        StoryDetails story)
    {
        if (stringBuilder is null)
            throw new ArgumentNullException(nameof(stringBuilder));
        if (titleTemplate is null)
            throw new ArgumentNullException(nameof(titleTemplate));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        stringBuilder.AppendLine(await _messageBuilder.Build(titleTemplate, languageId, story.Title));

        if (story.Links.Any())
        {
            foreach (var link in story.Links)
                stringBuilder.AppendLine(link);
        }
    }

    public void AddEstimates(
        StringBuilder stringBuilder,
        IReadOnlyCollection<EstimateItemDetails> items,
        bool estimateEnded)
    {
        if (stringBuilder is null)
            throw new ArgumentNullException(nameof(stringBuilder));
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        string AddEstimate(EstimateItemDetails item) => estimateEnded ? item.DisplayValue : item.HasValue;

        stringBuilder.AppendLine();

        foreach (var item in items)
            stringBuilder.AppendLine($"{item.AppraiserName} {AddEstimate(item)}");
    }

    private async Task AddTotalEstimate(StringBuilder stringBuilder, LanguageId languageId, SummaryByStory summary)
    {
        if (stringBuilder is null)
            throw new ArgumentNullException(nameof(stringBuilder));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (summary is null)
            throw new ArgumentNullException(nameof(summary));

        stringBuilder.AppendLine();
        stringBuilder.AppendLine(await _messageBuilder.Build(Messages.TotalEstimate, languageId, summary.Total));
    }
}