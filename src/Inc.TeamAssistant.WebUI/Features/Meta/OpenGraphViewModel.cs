namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record OpenGraphViewModel(string Title, string Description, string ImageName)
{
    public static readonly OpenGraphViewModel Empty = new(string.Empty, string.Empty, string.Empty);
}