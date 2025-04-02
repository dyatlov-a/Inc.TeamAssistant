using System.Text;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Extensions;
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

        builder
            .AppendLine()
            .AppendLine(BuildLinkForDashboard(summary.TeamId, summary.LanguageId))
            .AppendLine()
            .AddItems(
                summary.Items,
                (sb, i) => sb.AppendLine($"{i.AppraiserName} {AddEstimate(summary.EstimateEnded, i)}"));

        if (summary.EstimateEnded)
            await AddEstimateSummary(builder, summary);
        if (summary.Accepted)
            await AddAcceptedValue(builder, summary);
        
        await AddRoundsInfo(builder, summary);
        
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

        var message = summary.EstimateEnded
            ? Messages.Appraiser_EndEstimate
            : Messages.Appraiser_NeedEstimate;
        var storyHeader = await _messageBuilder.Build(message, summary.LanguageId);
        
        builder
            .AppendLine(storyHeader)
            .AppendLine(summary.StoryTitle)
            .AddIfHasValue(summary.Url);
    }

    private async Task AddEstimateSummary(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        builder
            .AppendLine()
            .Append(await _messageBuilder.Build(Messages.Appraiser_MeanEstimate, summary.LanguageId))
            .Append(' ')
            .Append(summary.Mean)
            .AppendLine()
            .Append(await _messageBuilder.Build(Messages.Appraiser_MedianEstimate, summary.LanguageId))
            .Append(' ')
            .Append(summary.Median);
    }

    private async Task AddAcceptedValue(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        builder
            .AppendLine()
            .Append(await _messageBuilder.Build(Messages.Appraiser_AcceptedEstimate, summary.LanguageId))
            .Append(' ')
            .Append(summary.AcceptedValue);
    }

    private async Task AddTeamActions(StringBuilder builder, SummaryByStory summary, NotificationMessage notification)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(notification);
        
        foreach (var assessment in summary.Assessments)
        {
            var buttonText = await _messageBuilder.Build(
                GetAssessmentMessageId(summary.StoryType, assessment.Code),
                summary.LanguageId);
            var buttonCommand = $"{string.Format(CommandList.Set, assessment.Value)}{summary.StoryId:N}";
                
            notification.WithButton(new Button(buttonText, buttonCommand));
        }

        var finishText = await _messageBuilder.Build(Messages.Appraiser_Finish, summary.LanguageId);
        var finishCommand = $"{CommandList.Finish}{summary.StoryId:N}";
        
        notification.WithButton(new Button(finishText, finishCommand));
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
                GetAssessmentMessageId(summary.StoryType, assessment.Code),
                summary.LanguageId);
            var buttonText = $"{acceptText} {buttonAssessmentText}";
            var buttonCommand = $"{string.Format(CommandList.AcceptEstimate, assessment.Value)}{summary.StoryId:N}";

            notification.WithButton(new Button(buttonText, buttonCommand));
        }
            
        var revoteText = await _messageBuilder.Build(Messages.Appraiser_Revote, summary.LanguageId);
        var revoteCommand = $"{CommandList.Revote}{summary.StoryId:N}";
        
        notification.WithButton(new Button(revoteText, revoteCommand));
    }
    
    private string BuildLinkForDashboard(Guid teamId, LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(languageId);
        
        var result = string.Format(
            _connectToDashboardLinkTemplate,
            languageId.Value,
            teamId.ToLinkSegment());
        return result;
    }

    private async Task AddRoundsInfo(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        var roundsInfo = await _messageBuilder.Build(Messages.Appraiser_NumberOfRounds, summary.LanguageId);
        
        builder
            .AppendLine()
            .AppendLine($"{roundsInfo} {summary.RoundsCount}");
    }
    
    private static string AddEstimate(bool estimateEnded, EstimateItemDetails item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var result = estimateEnded
            ? item.DisplayValue
            : item.HasValue;
        return result;
    }

    private static MessageId GetAssessmentMessageId(string storyType, string assessmentCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storyType);
        ArgumentException.ThrowIfNullOrWhiteSpace(assessmentCode);
        
        var messageId = $"Appraiser_{storyType}_{assessmentCode}";
        return new(messageId);
    }
}