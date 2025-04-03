namespace Inc.TeamAssistant.Gateway.Services;

public interface IMessageProvider
{
    Dictionary<string, Dictionary<string, string>> Get();
}