namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed record TeammatesPool(
    IReadOnlyCollection<long> Teammates,
    long OwnerId,
    long? ReviewerId,
    long? LastReviewerId)
{
    public IReadOnlyCollection<long> WithoutParticipates()
        => Apply(t => t != OwnerId && t != ReviewerId && t != LastReviewerId);

    public IReadOnlyCollection<long> OnlyOtherTeammates()
    {
        var teammatesWithoutOwner = WithoutOwner();
        var results = teammatesWithoutOwner.Any() ? teammatesWithoutOwner : WithoutReviewer();
        return results;
    }
    
    private IReadOnlyCollection<long> WithoutOwner()
        => Apply(t => t != OwnerId && t != ReviewerId);
    private IReadOnlyCollection<long> WithoutReviewer()
        => Apply(t => t != ReviewerId);
    
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