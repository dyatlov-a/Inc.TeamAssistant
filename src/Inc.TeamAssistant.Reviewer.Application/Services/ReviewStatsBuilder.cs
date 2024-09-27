using System.Text;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewStatsBuilder
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly StringBuilder _builder;
    
    private bool _hasReviewMetrics;
    private bool _hasCorrectionMetrics;

    private ReviewStatsBuilder(IMessageBuilder messageBuilder, StringBuilder builder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public static ReviewStatsBuilder Create(IMessageBuilder messageBuilder)
    {
        ArgumentNullException.ThrowIfNull(messageBuilder);
        
        return new ReviewStatsBuilder(messageBuilder, new StringBuilder());
    }
    
    public ReviewStatsBuilder WithReviewMetrics()
    {
        _hasReviewMetrics = true;
        
        return this;
    }
    
    public ReviewStatsBuilder WithCorrectionMetrics()
    {
        _hasCorrectionMetrics = true;
        
        return this;
    }

    public async Task<string> Build(
        TaskForReview task,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        ArgumentNullException.ThrowIfNull(languageId);

        _builder.AppendLine();
        var attempts = task.GetAttempts();
        if (attempts.HasValue)
            _builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsAttempts,
                languageId,
                attempts.Value));
        
        if (task.State == TaskForReviewState.Accept)
            await ByAccept(metricsByTeam, metricsByTask, languageId, attempts);
        else
            await ByInProgress(metricsByTeam, languageId, attempts);

        return _builder.ToString();
    }

    private async Task ByInProgress(ReviewTeamMetrics metricsByTeam, LanguageId languageId, int? attempts)
    {
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(languageId);
        
        if (_hasReviewMetrics)
        {
            _builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsFirstTouchAverage,
                languageId,
                metricsByTeam.FirstTouch.ToString(GlobalSettings.TimeFormat)));
            _builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsReviewAverage,
                languageId,
                metricsByTeam.Review.ToString(GlobalSettings.TimeFormat)));
        }

        if (_hasCorrectionMetrics && attempts.HasValue)
            _builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsCorrectionAverage,
                languageId,
                metricsByTeam.Correction.ToString(GlobalSettings.TimeFormat)));
    }

    private async Task ByAccept(
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        LanguageId languageId,
        int? attempts)
    {
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        ArgumentNullException.ThrowIfNull(languageId);
        
        if (_hasReviewMetrics)
        {
            var firstTouchTrend = metricsByTask.FirstTouch <= metricsByTeam.FirstTouch
                ? Icons.TrendUp
                : Icons.TrendDown;
            var firstTouchMessage = await _messageBuilder.Build(
                Messages.Reviewer_StatsFirstTouch,
                languageId,
                metricsByTask.FirstTouch.ToString(GlobalSettings.TimeFormat),
                metricsByTeam.FirstTouch.ToString(GlobalSettings.TimeFormat));
            _builder.AppendLine($"{firstTouchMessage} {firstTouchTrend}");
                
            var reviewTrend = metricsByTask.Review <= metricsByTeam.Review
                ? Icons.TrendUp
                : Icons.TrendDown;
            var reviewMessage = await _messageBuilder.Build(
                Messages.Reviewer_StatsReview,
                languageId,
                metricsByTask.Review.ToString(GlobalSettings.TimeFormat),
                metricsByTeam.Review.ToString(GlobalSettings.TimeFormat));
            _builder.AppendLine($"{reviewMessage} {reviewTrend}");
        }

        if (_hasCorrectionMetrics && attempts.HasValue)
        {
            var correctionTrend = metricsByTask.Correction <= metricsByTeam.Correction
                ? Icons.TrendUp
                : Icons.TrendDown;
            var correctionMessage = await _messageBuilder.Build(
                Messages.Reviewer_StatsCorrection,
                languageId,
                metricsByTask.Correction.ToString(GlobalSettings.TimeFormat),
                metricsByTeam.Correction.ToString(GlobalSettings.TimeFormat));
            _builder.AppendLine($"{correctionMessage} {correctionTrend}");
        }
    }
}