namespace Inc.TeamAssistant.Reviewer.Domain;

public enum NextReviewerType
{
    RoundRobin = 1,
    RoundRobinForTeam = 2,
    SecondRoundRobinForTeam = 3,
    Random = 11,
    Target = 100
}