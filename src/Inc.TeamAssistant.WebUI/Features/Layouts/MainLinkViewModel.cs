namespace Inc.TeamAssistant.WebUI.Features.Layouts;

public sealed record MainLinkViewModel(
    string Title,
    string Url,
    bool External = false);