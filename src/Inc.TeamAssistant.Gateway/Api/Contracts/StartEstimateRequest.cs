namespace Inc.TeamAssistant.Gateway.Api.Contracts;

public sealed record StartEstimateRequest(
    string AccessToken,
    string ProjectKey,
    string IssueKey,
    string IssueUrl);