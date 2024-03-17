using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.RandomCoffee.Application;

internal static class Messages
{
    public static readonly MessageId RandomCoffee_Question = new(nameof(RandomCoffee_Question));
    public static readonly MessageId RandomCoffee_Yes = new(nameof(RandomCoffee_Yes));
    public static readonly MessageId RandomCoffee_No = new(nameof(RandomCoffee_No));
    public static readonly MessageId RandomCoffee_SelectedPairs = new(nameof(RandomCoffee_SelectedPairs));
    public static readonly MessageId RandomCoffee_NotSelectedPair = new(nameof(RandomCoffee_NotSelectedPair));
    public static readonly MessageId RandomCoffee_MeetingDescription = new(nameof(RandomCoffee_MeetingDescription));
    public static readonly MessageId RandomCoffee_NotEnoughParticipants = new(nameof(RandomCoffee_NotEnoughParticipants));
}