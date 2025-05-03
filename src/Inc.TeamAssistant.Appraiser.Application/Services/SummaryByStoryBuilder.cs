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

    public NotificationMessage Build(SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        var notification = NotificationBuilder.Create()
            .Add(sb => Body(sb, summary))
            .AddIfHasValue(summary.Url, sb => sb.AppendLine(summary.Url))
            .Add(sb => sb
                .AppendLine()
                .AppendLine(DashboardLink(summary.TeamId, summary.LanguageId))
                .AppendLine())
            .AddEach(summary.Items, (sb, i) => Estimate(sb, summary.EstimateEnded, i))
            .AddIf(summary.EstimateEnded, sb => EstimateSummary(sb, summary))
            .AddIf(summary.Accepted, sb => AcceptedValue(sb, summary))
            .Add(sb => RoundsInformation(sb, summary))
            .Build(m => summary.StoryExternalId.HasValue
                ? NotificationMessage.Edit(new ChatMessage(summary.ChatId, summary.StoryExternalId.Value), m)
                : NotificationMessage.Create(summary.ChatId, m)
                    .WithHandler((c, mId, pId) => new AttachStoryCommand(c, summary.StoryId, mId)))
            .AddIf(!summary.Accepted, n => n.AddIfElse(
                summary.EstimateEnded,
                tn => OwnerActions(summary, tn),
                fn => TeamActions(summary, fn)));

        return notification;
    }

    private void Body(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);

        var message = summary.EstimateEnded
            ? Messages.Appraiser_EndEstimate
            : Messages.Appraiser_NeedEstimate;
        
        builder
            .AppendLine(_messageBuilder.Build(message, summary.LanguageId))
            .AppendLine(summary.StoryTitle);
    }

    private void EstimateSummary(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        builder
            .AppendLine()
            .Append(_messageBuilder.Build(Messages.Appraiser_MeanEstimate, summary.LanguageId))
            .AddSeparator()
            .Append(summary.Mean)
            .AppendLine()
            .Append(_messageBuilder.Build(Messages.Appraiser_MedianEstimate, summary.LanguageId))
            .AddSeparator()
            .Append(summary.Median);
    }

    private void AcceptedValue(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        builder
            .AppendLine()
            .Append(_messageBuilder.Build(Messages.Appraiser_AcceptedEstimate, summary.LanguageId))
            .AddSeparator()
            .Append(summary.AcceptedValue);
    }

    private void TeamActions(SummaryByStory summary, NotificationMessage notification)
    {
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(notification);
        
        foreach (var assessment in summary.Assessments)
        {
            var assessmentText = _messageBuilder.Build(
                AssessmentMessageId(summary.StoryType, assessment.Code),
                summary.LanguageId);
            var assessmentCommand = $"{string.Format(CommandList.Set, assessment.Value)}{summary.StoryId:N}";
                
            notification.WithButton(new Button(assessmentText, assessmentCommand));
        }

        var finishText = _messageBuilder.Build(Messages.Appraiser_Finish, summary.LanguageId);
        var finishCommand = $"{CommandList.Finish}{summary.StoryId:N}";
        
        notification.WithButton(new Button(finishText, finishCommand));
    }

    private void OwnerActions(SummaryByStory summary, NotificationMessage notification)
    {
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(notification);
        
        foreach (var assessment in summary.AssessmentsToAccept)
        {
            var acceptText = _messageBuilder.Build(
                Messages.Appraiser_Accept,
                summary.LanguageId);
            var assessmentPartText = _messageBuilder.Build(
                AssessmentMessageId(summary.StoryType, assessment.Code),
                summary.LanguageId);
            var assessmentText = $"{acceptText} {assessmentPartText}";
            var assessmentCommand = $"{string.Format(CommandList.AcceptEstimate, assessment.Value)}{summary.StoryId:N}";

            notification.WithButton(new Button(assessmentText, assessmentCommand));
        }
            
        var revoteText = _messageBuilder.Build(Messages.Appraiser_Revote, summary.LanguageId);
        var revoteCommand = $"{CommandList.Revote}{summary.StoryId:N}";
        
        notification.WithButton(new Button(revoteText, revoteCommand));
    }
    
    private string DashboardLink(Guid teamId, LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(languageId);
        
        var result = string.Format(
            _connectToDashboardLinkTemplate,
            languageId.Value,
            teamId.ToLinkSegment());
        
        return result;
    }

    private void RoundsInformation(StringBuilder builder, SummaryByStory summary)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        
        var roundsInfo = _messageBuilder.Build(Messages.Appraiser_NumberOfRounds, summary.LanguageId);
        
        builder
            .AppendLine()
            .AppendLine($"{roundsInfo} {summary.RoundsCount}");
    }
    
    private static void Estimate(StringBuilder builder, bool estimateEnded, EstimateItemDetails item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var estimate = estimateEnded
            ? item.DisplayValue
            : item.HasValue;

        builder.AppendLine($"{item.AppraiserName} {estimate}");
    }

    private static MessageId AssessmentMessageId(string storyType, string assessmentCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storyType);
        ArgumentException.ThrowIfNullOrWhiteSpace(assessmentCode);
        
        var messageId = $"Appraiser_{storyType}_{assessmentCode}";
        
        return new(messageId);
    }
}