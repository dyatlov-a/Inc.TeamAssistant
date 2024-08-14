namespace Inc.TeamAssistant.Gateway.Services.Integrations;

public sealed record StartEstimateRequest(
    string AccessToken,
    string ProjectKey,
    string IssueKey,
    string IssueUrl,
    string Subject);