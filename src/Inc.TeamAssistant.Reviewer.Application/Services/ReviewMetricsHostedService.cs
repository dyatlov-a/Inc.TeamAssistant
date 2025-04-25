using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMetricsHostedService : IHostedService
{
    private readonly ITaskForReviewReader _reader;
    private readonly IReviewMetricsLoader _metricsLoader;
    private readonly ILogger<ReviewMetricsHostedService> _logger;

    public ReviewMetricsHostedService(
        ITaskForReviewReader reader,
        IReviewMetricsLoader metricsLoader,
        ILogger<ReviewMetricsHostedService> logger)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _metricsLoader = metricsLoader ?? throw new ArgumentNullException(nameof(metricsLoader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken token)
    {
        try
        {
            var fromDate = DateTimeOffset.UtcNow.AddDays(-90);

            var taskForReviews = await _reader.GetTasksByTeam(teamId: null, fromDate, token);

            await _metricsLoader.Load(taskForReviews, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not load review stats");
        }
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}