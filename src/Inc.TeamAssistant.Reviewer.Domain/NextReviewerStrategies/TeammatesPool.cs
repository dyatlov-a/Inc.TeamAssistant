namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed record TeammatesPool(
    IReadOnlyCollection<long> Teammates,
    long OwnerId,
    long? ReviewerId,
    long? LastReviewerId)
{
    public IReadOnlyCollection<long> WithoutAllParticipants()
        => Apply(t => t != OwnerId && t != ReviewerId && t != LastReviewerId);

    public IReadOnlyCollection<long> WithoutPriorityParticipants()
    {
        var teammatesWithoutOwner = Apply(t => t != OwnerId && t != ReviewerId);
        var results = teammatesWithoutOwner.Any() ? teammatesWithoutOwner : Apply(t => t != ReviewerId);
        return results;
    }
    
    private IReadOnlyCollection<long> Apply(Func<long, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        
        var teammates = Teammates
            .Where(filter)
            .OrderBy(t => t)
            .ToArray();
        
        return teammates;
    }
}