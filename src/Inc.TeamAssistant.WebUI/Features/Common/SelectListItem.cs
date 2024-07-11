namespace Inc.TeamAssistant.WebUI.Features.Common;

public sealed record SelectListItem
{
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
}