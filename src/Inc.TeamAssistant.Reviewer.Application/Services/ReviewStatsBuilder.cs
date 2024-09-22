using System.Text;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewStatsBuilder
{
    private readonly IMessageBuilder _messageBuilder;

    public ReviewStatsBuilder(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<StringBuilder> Build(
        TaskForReview taskForReview,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        LanguageId languageId,
        bool hasReviewMetrics,
        bool hasCorrectionMetrics)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        ArgumentNullException.ThrowIfNull(languageId);

        var builder = new StringBuilder();

        builder.AppendLine();
        var attempts = taskForReview.GetAttempts();
        if (attempts.HasValue)
            builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsAttempts,
                languageId,
                attempts.Value));
        
        if (taskForReview.State == TaskForReviewState.Accept)
        {
            if (hasReviewMetrics)
            {
                var firstTouchTrend = metricsByTask.FirstTouch <= metricsByTeam.FirstTouch
                    ? Icons.TrendUp
                    : Icons.TrendDown;
                var firstTouchMessage = await _messageBuilder.Build(
                    Messages.Reviewer_StatsFirstTouch,
                    languageId,
                    metricsByTask.FirstTouch.ToString(GlobalSettings.TimeFormat),
                    metricsByTeam.FirstTouch.ToString(GlobalSettings.TimeFormat));
                builder.AppendLine($"{firstTouchMessage} {firstTouchTrend}");
                
                var reviewTrend = metricsByTask.Review <= metricsByTeam.Review
                    ? Icons.TrendUp
                    : Icons.TrendDown;
                var reviewMessage = await _messageBuilder.Build(
                    Messages.Reviewer_StatsReview,
                    languageId,
                    metricsByTask.Review.ToString(GlobalSettings.TimeFormat),
                    metricsByTeam.Review.ToString(GlobalSettings.TimeFormat));
                builder.AppendLine($"{reviewMessage} {reviewTrend}");
            }

            if (hasCorrectionMetrics && attempts.HasValue)
            {
                var correctionTrend = metricsByTask.Correction <= metricsByTeam.Correction
                    ? Icons.TrendUp
                    : Icons.TrendDown;
                var correctionMessage = await _messageBuilder.Build(
                    Messages.Reviewer_StatsCorrection,
                    languageId,
                    metricsByTask.Correction.ToString(GlobalSettings.TimeFormat),
                    metricsByTeam.Correction.ToString(GlobalSettings.TimeFormat));
                builder.AppendLine($"{correctionMessage} {correctionTrend}");
            }
        }
        else
        {
            if (hasReviewMetrics)
            {
                builder.AppendLine(await _messageBuilder.Build(
                    Messages.Reviewer_StatsFirstTouchAverage,
                    languageId,
                    metricsByTeam.FirstTouch.ToString(GlobalSettings.TimeFormat)));
                builder.AppendLine(await _messageBuilder.Build(
                    Messages.Reviewer_StatsReviewAverage,
                    languageId,
                    metricsByTeam.Review.ToString(GlobalSettings.TimeFormat)));
            }
            
            if (hasCorrectionMetrics && attempts.HasValue)
                builder.AppendLine(await _messageBuilder.Build(
                    Messages.Reviewer_StatsCorrectionAverage,
                    languageId,
                    metricsByTeam.Correction.ToString(GlobalSettings.TimeFormat)));
        }

        return builder;
    }
}