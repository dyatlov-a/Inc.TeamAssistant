using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Domain;

public static class TeamProperties
{
    public const string NextReviewerTypeKey = "nextReviewerStrategy";
    public const string WaitingNotificationIntervalKey = "waitingNotificationInterval";
    public const string InProgressNotificationIntervalKey = "inProgressNotificationInterval";
    
    private static readonly string NextReviewerTypeDefault = NextReviewerType.RoundRobin.ToString();
    private static readonly TimeSpan WaitingNotificationIntervalDefault = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan InProgressNotificationIntervalDefault = TimeSpan.FromHours(1);

    public static string GetNextReviewerType(this CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(teamContext);
        
        return teamContext.Properties.GetValueOrDefault(NextReviewerTypeKey, NextReviewerTypeDefault);
    }

    public static NotificationIntervals GetNotificationIntervals(this BotContext botContext)
    {
        ArgumentNullException.ThrowIfNull(botContext);
        
        var waiting = botContext.GetIntervalOrDefault(
            WaitingNotificationIntervalKey,
            WaitingNotificationIntervalDefault);
        var inProgress = botContext.GetIntervalOrDefault(
            InProgressNotificationIntervalKey,
            InProgressNotificationIntervalDefault);

        return new(waiting, inProgress);
    }
}