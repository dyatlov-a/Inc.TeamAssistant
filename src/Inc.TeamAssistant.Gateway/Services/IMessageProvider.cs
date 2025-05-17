namespace Inc.TeamAssistant.Gateway.Services;

public interface IMessageProvider
{
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Data { get; }
}