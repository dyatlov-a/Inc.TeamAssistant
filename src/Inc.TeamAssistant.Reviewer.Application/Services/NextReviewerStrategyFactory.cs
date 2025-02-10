using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class NextReviewerStrategyFactory : INextReviewerStrategyFactory
{
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly ITaskForReviewRepository _taskForReviewRepository;

    public NextReviewerStrategyFactory(
        ITaskForReviewReader taskForReviewReader,
        ITaskForReviewRepository taskForReviewRepository)
    {
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
    }

    public async Task<INextReviewerStrategy> Create(
        Guid teamId,
        long ownerId,
        NextReviewerType nextReviewerType,
        long? targetPersonId,
        IReadOnlyCollection<long> teammates,
        long? excludePersonId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(teammates);
        
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(
            teamId,
            ownerId,
            token);
        var history = await _taskForReviewReader.GetHistory(
            teamId,
            DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday),
            token);
        
        var excludedPersonIds = excludePersonId.HasValue
            ? new[] { ownerId, excludePersonId.Value }
            : [ownerId];
        
        return nextReviewerType switch
        {
            NextReviewerType.RoundRobin => new RoundRobinReviewerStrategy(teammates, excludedPersonIds, lastReviewerId),
            NextReviewerType.Random => new RandomReviewerStrategy(teammates, history, excludedPersonIds, lastReviewerId),
            NextReviewerType.Target when targetPersonId.HasValue => new TargetReviewerStrategy(targetPersonId.Value),
            _ => throw new TeamAssistantException($"NextReviewerType {nextReviewerType} was not supported.")
        };
    }
}