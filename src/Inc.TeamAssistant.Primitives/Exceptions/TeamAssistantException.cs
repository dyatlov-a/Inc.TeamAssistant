namespace Inc.TeamAssistant.Primitives.Exceptions;

public class TeamAssistantException : ApplicationException
{
    public TeamAssistantException(string message)
        : base(message)
    {
    }
}