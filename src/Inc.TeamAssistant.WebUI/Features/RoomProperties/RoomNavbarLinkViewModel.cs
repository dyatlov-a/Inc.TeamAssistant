namespace Inc.TeamAssistant.WebUI.Features.RoomProperties;

public sealed record RoomNavbarLinkViewModel(
    string TitleKey,
    string Url,
    bool Selected);