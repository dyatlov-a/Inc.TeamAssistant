using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks.Converters;

internal sealed class TaskForReviewHistoryConverter
{
    private readonly ReviewTeamMetricsFactory _metricsFactory;

    public TaskForReviewHistoryConverter(ReviewTeamMetricsFactory metricsFactory)
    {
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
    }

    public async Task<TaskForReviewDto> ConvertTo(TaskForReviewHistory item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);

        var stats = await _metricsFactory.Create(item, token);
        
        return new TaskForReviewDto(
            item.Id,
            item.Created,
            item.State.ToString(),
            item.Description,
            stats.FirstTouch,
            stats.Correction,
            stats.Review,
            stats.Iterations,
            item.ReviewerId,
            item.ReviewerName,
            item.ReviewerUserName,
            item.OwnerId,
            item.OwnerName,
            item.OwnerUserName,
            HasConcreteReviewer: item.Strategy == NextReviewerType.Target,
            item.IsOriginalReviewer);
    }
}