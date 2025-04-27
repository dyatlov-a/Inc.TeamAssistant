using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class WarmCacheHostedService : IHostedService
{
    private readonly ITaskForReviewReader _reader;
    private readonly IReviewMetricsLoader _metricsLoader;
    private readonly ReviewCommentsProvider _commentsProvider;
    private readonly ILogger<WarmCacheHostedService> _logger;

    public WarmCacheHostedService(
        ITaskForReviewReader reader,
        IReviewMetricsLoader metricsLoader,
        ReviewCommentsProvider commentsProvider,
        ILogger<WarmCacheHostedService> logger)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _metricsLoader = metricsLoader ?? throw new ArgumentNullException(nameof(metricsLoader));
        _commentsProvider = commentsProvider ?? throw new ArgumentNullException(nameof(commentsProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken token)
    {
        await LoadReviewMetrics(token);
        await LoadReviewComments(token);
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;

    private async Task LoadReviewMetrics(CancellationToken token)
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
    
    private async Task LoadReviewComments(CancellationToken token)
    {
        try
        {
            var tasks = await _reader.GetAll(TaskForReviewStateRules.ActiveStates, teamId: null, token);

            foreach (var taskForReview in tasks)
                _commentsProvider.Add(taskForReview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not load review comments");
        }
    }
}