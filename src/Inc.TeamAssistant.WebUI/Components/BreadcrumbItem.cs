namespace Inc.TeamAssistant.WebUI.Components;

public sealed record BreadcrumbItem(
    string Text,
    bool Selected,
    string Href);