using System.Text;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class SummaryByStoryBuilder
{
	private readonly IMessageBuilder _messageBuilder;
    private readonly string _connectToDashboardLinkTemplate;

    public SummaryByStoryBuilder(IMessageBuilder messageBuilder, string connectToDashboardLinkTemplate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectToDashboardLinkTemplate);
        
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _connectToDashboardLinkTemplate = connectToDashboardLinkTemplate;
    }

    public async Task<NotificationMessage> Build(SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        var builder = new StringBuilder();

        await AddBody(builder, summary);

        builder.AppendLine();
        builder.AppendLine(BuildLinkForDashboard(summary.TeamId, summary.LanguageId));

        builder.AppendLine();
        foreach (var item in summary.Items)
            builder.AppendLine($"{item.AppraiserName} {AddEstimate(summary.EstimateEnded, item)}");

        if (summary.EstimateEnded)
            await AddEstimateSummary(builder, summary);
        if (summary.Accepted)
            await AddAcceptedValue(builder, summary);
        
        var notification = summary.StoryExternalId.HasValue
            ? NotificationMessage.Edit(
                new ChatMessage(summary.ChatId, summary.StoryExternalId.Value),
                builder.ToString())
            : NotificationMessage
                .Create(summary.ChatId, builder.ToString())
                .AddHandler((c, p) => new AttachStoryCommand(c, summary.StoryId, int.Parse(p)));

        if (summary.Accepted)
            return notification;
        
        if (summary.EstimateEnded)
            await AddOwnerActions(builder, summary, notification);
        else
            await AddTeamActions(builder, summary, notification);

        return notification;
    }

    private async Task AddBody(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        var storyHeader = await _messageBuilder.Build(
            summary.EstimateEnded ? Messages.Appraiser_EndEstimate : Messages.Appraiser_NeedEstimate,
            summary.LanguageId);
        
        builder.AppendLine(storyHeader);
        builder.AppendLine(summary.StoryTitle);
        if (summary.StoryLinks.Any())
            foreach (var link in summary.StoryLinks)
                builder.AppendLine(link);
    }

    private async Task AddEstimateSummary(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        builder.AppendLine();
        builder.Append(await _messageBuilder.Build(Messages.Appraiser_MeanEstimate, summary.LanguageId));
        builder.Append(' ');
        builder.Append(summary.Mean);
            
        builder.AppendLine();
        builder.Append(await _messageBuilder.Build(Messages.Appraiser_MedianEstimate, summary.LanguageId));
        builder.Append(' ');
        builder.Append(summary.Median);
    }

    private async Task AddAcceptedValue(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        builder.AppendLine();
        builder.Append(await _messageBuilder.Build(Messages.Appraiser_AcceptedEstimate, summary.LanguageId));
        builder.Append(' ');
        builder.Append(summary.AcceptedValue);
    }

    private async Task AddTeamActions(StringBuilder builder, SummaryByStory summary, NotificationMessage notification)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(notification);
        
        foreach (var assessment in summary.Assessments)
        {
            var buttonText = await _messageBuilder.Build(
                new MessageId($"Appraiser_{summary.StoryType}_{assessment.Code}"),
                summary.LanguageId);
                
            notification.WithButton(new Button(
                buttonText,
                $"{string.Format(CommandList.Set, assessment.Value)}{summary.StoryId:N}"));
        }

        var finishText = await _messageBuilder.Build(Messages.Appraiser_Finish, summary.LanguageId);
        notification.WithButton(new Button(finishText, $"{CommandList.Finish}{summary.StoryId:N}"));
    }

    private async Task AddOwnerActions(StringBuilder builder, SummaryByStory summary, NotificationMessage notification)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(notification);
        
        foreach (var assessment in summary.AssessmentsToAccept)
        {
            var acceptText = await _messageBuilder.Build(
                Messages.Appraiser_Accept,
                summary.LanguageId);

            var buttonAssessmentText = await _messageBuilder.Build(
                new MessageId($"Appraiser_{summary.StoryType}_{assessment.Code}"),
                summary.LanguageId);

            notification.WithButton(new Button(
                $"{acceptText} {buttonAssessmentText}",
                $"{string.Format(CommandList.AcceptEstimate, assessment.Value)}{summary.StoryId:N}"));
        }
            
        var revoteText = await _messageBuilder.Build(Messages.Appraiser_Revote, summary.LanguageId);
        notification.WithButton(new Button(revoteText, $"{CommandList.Revote}{summary.StoryId:N}"));
    }

    private string AddEstimate(bool estimateEnded, EstimateItemDetails item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return estimateEnded ? item.DisplayValue : item.HasValue;
    }
    
    private string BuildLinkForDashboard(Guid teamId, LanguageId languageId)
    {
        return string.Format(
            _connectToDashboardLinkTemplate,
            languageId.Value,
            teamId.ToString("N"));
    }
}