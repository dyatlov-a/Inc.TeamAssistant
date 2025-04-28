using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks.Converters;

internal static class TaskForReviewHistoryConverter
{
    public static TaskForReviewDto ConvertTo(TaskForReviewHistory item, ReviewTeamMetrics stats)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(stats);

        var state = item.State.ToString();
        var hasConcreteReviewer = item.Strategy == NextReviewerType.Target;
        var hasReassign = item.OriginalReviewerId.HasValue && item.ReviewerId != item.OriginalReviewerId.Value;
        var comments = item.Comments
            .OrderBy(c => c.Created)
            .Select(c => c.Comment)
            .ToArray();
        
        return new TaskForReviewDto(
            item.Id,
            item.Created,
            state,
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
            hasConcreteReviewer,
            hasReassign,
            comments);
    }
}