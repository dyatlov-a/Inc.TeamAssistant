namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotListenerProvider
{
    IBotListener Listener { get; }
}