namespace Inc.TeamAssistant.Gateway.Services.Integrations;

public sealed record StartEstimateRequest(
    string Subject,
    string? IssueKey,
    string? IssueUrl);