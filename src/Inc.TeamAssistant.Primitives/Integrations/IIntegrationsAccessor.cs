namespace Inc.TeamAssistant.Primitives.Integrations;

public interface IIntegrationsAccessor
{
    Task<IntegrationContext?> Find(string accessToken, string projectKey, CancellationToken token);
}