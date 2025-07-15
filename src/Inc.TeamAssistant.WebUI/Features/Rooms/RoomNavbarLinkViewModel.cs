namespace Inc.TeamAssistant.WebUI.Features.Rooms;

public sealed record RoomNavbarLinkViewModel(
    string TitleKey,
    string Url,
    bool Selected);