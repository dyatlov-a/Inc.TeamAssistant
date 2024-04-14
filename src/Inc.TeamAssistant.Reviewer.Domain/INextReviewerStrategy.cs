namespace Inc.TeamAssistant.Reviewer.Domain;

public interface INextReviewerStrategy
{
    long Next(IReadOnlyCollection<long> excludedPersonIds, long? lastReviewerId);
}