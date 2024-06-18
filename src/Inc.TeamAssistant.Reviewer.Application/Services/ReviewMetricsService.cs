using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMetricsService : IHostedService
{
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly IReviewMetricsLoader _reviewMetricsLoader;
    private readonly ILogger<ReviewMetricsService> _logger;

    public ReviewMetricsService(
        ITaskForReviewReader taskForReviewReader,
        IReviewMetricsLoader reviewMetricsLoader,
        ILogger<ReviewMetricsService> logger)
    {
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _reviewMetricsLoader = reviewMetricsLoader ?? throw new ArgumentNullException(nameof(reviewMetricsLoader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken token)
    {
        try
        {
            var fromDate = DateTimeOffset.UtcNow.AddDays(-90);

            var taskForReviews = await _taskForReviewReader.GetTasksFrom(teamId: null, fromDate, token);

            await _reviewMetricsLoader.Load(taskForReviews, token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Can not load review stats");
        }
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}