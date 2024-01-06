using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class SummaryByStoryBuilder
{
	private readonly IMessageBuilder _messageBuilder;

    public SummaryByStoryBuilder(IMessageBuilder messageBuilder)
	{
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<NotificationMessage> Build(SummaryByStory summary)
    {
        if (summary is null)
            throw new ArgumentNullException(nameof(summary));

        var builder = new StringBuilder();

        builder.AppendLine(await _messageBuilder.Build(
            summary.EstimateEnded ? Messages.EndEstimate : Messages.NeedEstimate,
            summary.AssessmentSessionLanguageId,
            summary.StoryTitle));

        if (summary.StoryLinks.Any())
            foreach (var link in summary.StoryLinks)
                builder.AppendLine(link);

        builder.AppendLine();
        foreach (var item in summary.Items)
            builder.AppendLine($"{item.AppraiserName} {AddEstimate(summary.EstimateEnded, item)}");

        if (summary.EstimateEnded)
        {
            builder.AppendLine();
            builder.AppendLine(await _messageBuilder.Build(
                Messages.TotalEstimate,
                summary.AssessmentSessionLanguageId,
                summary.Total));
        }
        
        var notification = summary.StoryExternalId.HasValue
            ? NotificationMessage.Edit(
                new[] { new ChatMessage(summary.ChatId, summary.StoryExternalId.Value) },
                builder.ToString())
            : NotificationMessage
                .Create(summary.ChatId, builder.ToString())
                .AddHandler((_, uName, mId) => AddStoryForEstimate(summary.AssessmentSessionId, uName, mId));

        if (!summary.EstimateEnded)
            foreach (var assessment in AssessmentValue.GetAssessments)
            {
                var value = assessment.ToString();
            
                notification.WithButton(
                    new Button(value.Replace("sp", string.Empty, StringComparison.InvariantCultureIgnoreCase),
                        value));
            }

        return notification;
    }

    private IRequest<CommandResult> AddStoryForEstimate(Guid assessmentSessionId, string userName, int messageId)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        return new AddStoryForEstimateCommand(assessmentSessionId, userName, messageId);
    }

    private string AddEstimate(bool estimateEnded, EstimateItemDetails item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));
        
        return estimateEnded ? item.DisplayValue : item.HasValue;
    }
}