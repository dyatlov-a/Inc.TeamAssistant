namespace Inc.TeamAssistant.Primitives.Exceptions;

public sealed record ErrorDetails(
    string Type,
    string Title,
    int Status,
    string TraceId,
    IDictionary<string, string[]> Errors);