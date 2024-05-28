using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewAverageStatsService : IHostedService
{
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly IReviewAverageStatsProvider _reviewAverageStatsProvider;
    private readonly ILogger<ReviewAverageStatsService> _logger;

    public ReviewAverageStatsService(
        ITaskForReviewReader taskForReviewReader,
        IReviewAverageStatsProvider reviewAverageStatsProvider,
        ILogger<ReviewAverageStatsService> logger)
    {
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _reviewAverageStatsProvider = reviewAverageStatsProvider ?? throw new ArgumentNullException(nameof(reviewAverageStatsProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken token)
    {
        try
        {
            var fromDate = DateTimeOffset.UtcNow.AddDays(-90);

            var taskForReviews = await _taskForReviewReader.GetTasksFrom(fromDate, token);

            _reviewAverageStatsProvider.Initialize(taskForReviews);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Can not load review stats");
        }
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}