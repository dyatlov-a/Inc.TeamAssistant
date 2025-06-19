namespace Inc.TeamAssistant.WebUI.Components;

public sealed record NavigationItem<T>(
    string Text,
    T Value,
    bool Selected,
    bool CanMove);