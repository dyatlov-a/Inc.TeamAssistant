using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class NextReviewerStrategyFactory : INextReviewerStrategyFactory
{
    private readonly ITaskForReviewReader _reader;
    private readonly ITaskForReviewRepository _repository;

    public NextReviewerStrategyFactory(ITaskForReviewReader reader, ITaskForReviewRepository repository)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
        
        var lastDayOfWeek = DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday);
        var lastReviewerId = await _repository.FindLastReviewer(teamId, ownerId, token);
        var history = await _reader.GetHistory(teamId, lastDayOfWeek, token);
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