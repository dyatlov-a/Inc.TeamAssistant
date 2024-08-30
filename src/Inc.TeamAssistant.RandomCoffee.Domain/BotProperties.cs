using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.RandomCoffee.Domain;

public static class BotProperties
{
    public const string RoundIntervalKey = "roundInterval";
    public const string VotingIntervalKey = "votingInterval";

    public static TimeSpan GetRoundInterval(this BotContext context) => context.GetIntervalOrDefault(
        RoundIntervalKey,
        TimeSpan.FromDays(14));
    
    public static TimeSpan GetVotingInterval(this BotContext context) => context.GetIntervalOrDefault(
        VotingIntervalKey,
        TimeSpan.FromDays(1));
}