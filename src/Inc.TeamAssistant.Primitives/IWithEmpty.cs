namespace Inc.TeamAssistant.Primitives;

public interface IWithEmpty<out T>
{
    static abstract T Empty { get; }
}