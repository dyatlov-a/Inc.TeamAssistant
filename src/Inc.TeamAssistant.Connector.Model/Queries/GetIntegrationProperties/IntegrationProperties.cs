namespace Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;

public sealed record IntegrationProperties(
    string AccessToken,
    string ProjectKey,
    long ScrumMasterId);