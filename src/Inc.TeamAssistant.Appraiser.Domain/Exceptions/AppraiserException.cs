namespace Inc.TeamAssistant.Appraiser.Domain.Exceptions;

public class AppraiserException : ApplicationException
{
    public AppraiserException(string message)
        : base(message)
    {
    }
}