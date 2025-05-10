namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed record RetroColumnViewModel(
    string Name,
    int Type,
    string ItemColor,
    List<RetroItemViewModel> Items);