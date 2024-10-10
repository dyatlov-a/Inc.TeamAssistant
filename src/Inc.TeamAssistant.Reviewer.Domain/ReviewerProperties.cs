using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Domain;

public static class ReviewerProperties
{
    public const string NextReviewerTypeKey = "nextReviewerStrategy";
    public const string WaitingNotificationIntervalKey = "waitingNotificationInterval";
    public const string InProgressNotificationIntervalKey = "inProgressNotificationInterval";
    public const string AcceptWithCommentsKey = "acceptWithComments";

    public static string GetNextReviewerType(this CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(teamContext);
        
        return teamContext.Properties.GetValueOrDefault(NextReviewerTypeKey, NextReviewerType.RoundRobin.ToString());
    }

    public static NotificationIntervals GetNotificationIntervals(this BotContext botContext)
    {
        ArgumentNullException.ThrowIfNull(botContext);
        
        var waiting = botContext.GetIntervalOrDefault(WaitingNotificationIntervalKey, TimeSpan.FromMinutes(30));
        var inProgress = botContext.GetIntervalOrDefault(InProgressNotificationIntervalKey, TimeSpan.FromHours(1));

        return new(waiting, inProgress);
    }

    public static bool CanAcceptWithComments(this BotContext botContext)
    {
        ArgumentNullException.ThrowIfNull(botContext);

        var canAcceptWithComments = botContext.Properties.GetValueOrDefault(AcceptWithCommentsKey);
        bool.TryParse(canAcceptWithComments, out var result);

        return result;
    }
}