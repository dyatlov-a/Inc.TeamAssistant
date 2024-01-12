using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class SummaryByStoryBuilder
{
	private readonly IMessageBuilder _messageBuilder;
    private readonly ILinkBuilder _linkBuilder;

    public SummaryByStoryBuilder(IMessageBuilder messageBuilder, ILinkBuilder linkBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
    }

    public async Task<NotificationMessage> Build(SummaryByStory summary)
    {
        if (summary is null)
            throw new ArgumentNullException(nameof(summary));

        var builder = new StringBuilder();

        builder.AppendLine(await _messageBuilder.Build(
            summary.EstimateEnded ? Messages.Appraiser_EndEstimate : Messages.Appraiser_NeedEstimate,
            summary.LanguageId,
            summary.StoryTitle));

        if (summary.StoryLinks.Any())
            foreach (var link in summary.StoryLinks)
                builder.AppendLine(link);

        builder.AppendLine();

        builder.AppendLine(_linkBuilder.BuildLinkForDashboard(summary.TeamId, summary.LanguageId));

        builder.AppendLine();
        foreach (var item in summary.Items)
            builder.AppendLine($"{item.AppraiserName} {AddEstimate(summary.EstimateEnded, item)}");

        if (summary.EstimateEnded)
        {
            builder.AppendLine();
            builder.AppendLine(await _messageBuilder.Build(
                Messages.Appraiser_TotalEstimate,
                summary.LanguageId,
                summary.Total));
        }
        
        var notification = summary.StoryExternalId.HasValue
            ? NotificationMessage.Edit(
                new[] { new ChatMessage(summary.ChatId, summary.StoryExternalId.Value, Shared: false) },
                builder.ToString())
            : NotificationMessage
                .Create(summary.ChatId, builder.ToString())
                .AddHandler((c, mId) => new AttachStoryCommand(c, summary.StoryId, mId));

        if (!summary.EstimateEnded)
        {
            foreach (var assessment in AssessmentValue.GetAssessments)
            {
                var value = assessment.ToString();
            
                notification.WithButton(
                    new Button(value.Replace("sp", string.Empty, StringComparison.InvariantCultureIgnoreCase),
                        $"/{value}?storyId={summary.StoryId:N}"));
            }

            notification.WithButton(new Button("Accept", $"/accept?storyId={summary.StoryId:N}"));
        }
        else
            notification.WithButton(new Button("Revote", $"/revote?storyId={summary.StoryId:N}"));

        return notification;
    }

    private string AddEstimate(bool estimateEnded, EstimateItemDetails item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));
        
        return estimateEnded ? item.DisplayValue : item.HasValue;
    }
}