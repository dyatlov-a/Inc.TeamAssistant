using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.RandomCoffee.Domain;

public static class Messages
{
    public static readonly MessageId RandomCoffee_AlreadyWaitAnswer = new(nameof(RandomCoffee_AlreadyWaitAnswer));
    public static readonly MessageId Connector_HasNoRights = new(nameof(Connector_HasNoRights));
}