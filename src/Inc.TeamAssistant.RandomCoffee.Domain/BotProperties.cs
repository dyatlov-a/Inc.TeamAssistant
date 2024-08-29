using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.RandomCoffee.Domain;

public static class BotProperties
{
    public const string RoundIntervalKey = "roundInterval";
    public const string VotingIntervalKey = "votingInterval";
    
    private static readonly TimeSpan RoundIntervalDefault = TimeSpan.FromDays(14);
    private static readonly TimeSpan VotingIntervalDefault = TimeSpan.FromDays(1);

    public static TimeSpan GetRoundInterval(this BotContext context) => context.GetIntervalOrDefault(
        RoundIntervalKey,
        RoundIntervalDefault);
    
    public static TimeSpan GetVotingInterval(this BotContext context) => context.GetIntervalOrDefault(
        VotingIntervalKey,
        VotingIntervalDefault);
}