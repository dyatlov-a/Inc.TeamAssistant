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
	private readonly string _setCommand;
	private readonly string _noIdeaCommand;

	public SummaryByStoryBuilder(IMessageBuilder messageBuilder, string setCommand, string noIdeaCommand)
	{
		if (string.IsNullOrWhiteSpace(setCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(setCommand));
		if (string.IsNullOrWhiteSpace(noIdeaCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(noIdeaCommand));

		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
		_setCommand = setCommand;
		_noIdeaCommand = noIdeaCommand;
	}

	public async Task<NotificationMessage> Build(LanguageId languageId, bool estimateEnded, SummaryByStory summary)
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
        else
            await AddAssessments(builder, languageId);

		var messageText = builder.ToString();

        return estimateEnded
            ? NotificationMessage.Create(summary.ChatId, messageText)
            : NotificationMessage.Edit(
                summary.Items.Select(i => (i.AppraiserId.Value, i.StoryExternalId)).ToArray(),
                messageText);
    }

	public async Task AddAssessments(StringBuilder stringBuilder, LanguageId languageId)
	{
		if (stringBuilder is null)
			throw new ArgumentNullException(nameof(stringBuilder));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));

        stringBuilder.AppendLine();
        stringBuilder.Append(await _messageBuilder.Build(Messages.EnterEstimate, languageId));

		foreach (var assessment in AssessmentValue.GetAssessments)
			stringBuilder.Append($" {_setCommand}{(int)assessment}");

		stringBuilder.Append($" {_noIdeaCommand}");
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