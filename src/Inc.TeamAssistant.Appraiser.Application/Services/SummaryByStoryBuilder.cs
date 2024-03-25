using System.Text;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
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

        if (!summary.EstimateEnded)
        {
            foreach (var assessment in summary.Assessments)
            {
                var buttonText = assessment
                    .Replace("sp", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .ToUpperInvariant();
                
                notification.WithButton(new Button(buttonText, string.Format(CommandList.Set, assessment) + summary.StoryId.ToString("N")));
            }

            notification.WithButton(new Button("Accept", CommandList.AcceptEstimate + summary.StoryId.ToString("N")));
        }
        else
            notification.WithButton(new Button("Revote", CommandList.ReVote + summary.StoryId.ToString("N")));

        return notification;
    }

    private string AddEstimate(bool estimateEnded, EstimateItemDetails item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));
        
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