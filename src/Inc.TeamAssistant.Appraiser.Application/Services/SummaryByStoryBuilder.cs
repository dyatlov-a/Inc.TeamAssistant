using System.Text;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class SummaryByStoryBuilder
{
	private readonly IMessageBuilder _messageBuilder;
    private readonly AppraiserOptions _options;

    public SummaryByStoryBuilder(IMessageBuilder messageBuilder, AppraiserOptions options)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<NotificationMessage> Build(SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        var builder = new StringBuilder();

        var storyHeader = await _messageBuilder.Build(
            summary.EstimateEnded ? Messages.Appraiser_EndEstimate : Messages.Appraiser_NeedEstimate,
            summary.LanguageId);
        builder.AppendLine(storyHeader);
        
        builder.AppendLine(summary.StoryTitle);
        if (summary.StoryLinks.Any())
            foreach (var link in summary.StoryLinks)
                builder.AppendLine(link);

        builder.AppendLine();

        builder.AppendLine(BuildLinkForDashboard(summary.TeamId, summary.LanguageId));

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
                new ChatMessage(summary.ChatId, summary.StoryExternalId.Value),
                builder.ToString())
            : NotificationMessage
                .Create(summary.ChatId, builder.ToString())
                .AddHandler((c, p) => new AttachStoryCommand(c, summary.StoryId, int.Parse(p)));
        
        if (summary.Accepted)
            return notification;
        
        if (!summary.EstimateEnded)
        {
            foreach (var assessment in summary.Assessments)
            {
                var buttonText = await _messageBuilder.Build(
                    new MessageId($"Appraiser_{assessment}"),
                    summary.LanguageId);
                
                notification.WithButton(new Button(
                    buttonText,
                    $"{string.Format(CommandList.Set, assessment)}{summary.StoryId:N}"));
            }

            var finishText = await _messageBuilder.Build(Messages.Appraiser_Finish, summary.LanguageId);
            notification.WithButton(new Button(finishText, $"{CommandList.Finish}{summary.StoryId:N}"));
        }
        else
        {
            foreach (var assessment in summary.AssessmentsToAccept)
            {
                var acceptText = await _messageBuilder.Build(
                    Messages.Appraiser_Accept,
                    summary.LanguageId);

                var buttonAssessmentText = await _messageBuilder.Build(
                    new MessageId($"Appraiser_{assessment}"),
                    summary.LanguageId);

                notification.WithButton(new Button(
                    $"{acceptText} {buttonAssessmentText}",
                    $"{string.Format(CommandList.AcceptEstimate, assessment)}{summary.StoryId:N}"));
            }
            
            var revoteText = await _messageBuilder.Build(Messages.Appraiser_Revote, summary.LanguageId);
            notification.WithButton(new Button(revoteText, $"{CommandList.Revote}{summary.StoryId:N}"));
        }

        return notification;
    }

    private string AddEstimate(bool estimateEnded, EstimateItemDetails item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return estimateEnded ? item.DisplayValue : item.HasValue;
    }
    
    private string BuildLinkForDashboard(Guid teamId, LanguageId languageId)
    {
        return string.Format(
            _options.ConnectToDashboardLinkTemplate,
            languageId.Value,
            teamId.ToString("N"));
    }
}