namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IMessageProvider
{
    Task<Dictionary<string, Dictionary<string, string>>> Get(CancellationToken token = default);
}