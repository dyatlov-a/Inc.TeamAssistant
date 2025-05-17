namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed record RetroColumnViewModel(
    Guid Id,
    string Name,
    int Position,
    string ItemColor,
    List<RetroItemViewModel> Items);