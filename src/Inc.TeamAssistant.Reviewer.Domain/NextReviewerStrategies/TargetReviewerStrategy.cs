namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed class TargetReviewerStrategy : INextReviewerStrategy
{
    private readonly long _targetPersonId;

    public TargetReviewerStrategy(long targetPersonId)
    {
        _targetPersonId = targetPersonId;
    }

    public long GetReviewer() => _targetPersonId;
}