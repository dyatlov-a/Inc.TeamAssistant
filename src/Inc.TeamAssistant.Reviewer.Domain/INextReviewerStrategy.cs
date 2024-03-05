namespace Inc.TeamAssistant.Reviewer.Domain;

public interface INextReviewerStrategy
{
    long Next(long ownerId, long? lastReviewerId);
}