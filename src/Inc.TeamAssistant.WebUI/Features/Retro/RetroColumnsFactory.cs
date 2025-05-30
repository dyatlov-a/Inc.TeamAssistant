namespace Inc.TeamAssistant.WebUI.Features.Retro;

internal static class RetroColumnsFactory
{
    public static IReadOnlyCollection<RetroColumnViewModel> Create()
    {
        return
        [
            new (Guid.Parse("290fb89e-93a6-485e-bcc2-a10af9fab1d1"), "Start", Position: 1, ItemColor: "#FF8C00"),
            new (Guid.Parse("01d48058-afbb-4beb-9b03-80075bed4c11"), "Stop", Position: 2, ItemColor: "#B22222"),
            new (Guid.Parse("99634086-7a5a-46ee-94de-2f7ecfab17aa"), "Continue", Position: 3, ItemColor: "#3CB371")
        ];
    }
}