namespace Inc.TeamAssistant.Reviewer.Domain;

public static class TeamProperties
{
    public const string NextReviewerTypeKey = "nextReviewerStrategy";
    
    public static readonly string NextReviewerTypeDefault = NextReviewerType.RoundRobin.ToString();
}