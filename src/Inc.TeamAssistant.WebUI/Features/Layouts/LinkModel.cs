namespace Inc.TeamAssistant.WebUI.Features.Layouts;

public sealed record LinkModel(
    string Title,
    string Url,
    bool External = false);