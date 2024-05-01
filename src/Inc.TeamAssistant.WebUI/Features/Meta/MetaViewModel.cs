namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record MetaViewModel(string Title, string Description, string Keywords, string Author)
{
    public static readonly MetaViewModel Empty = new(string.Empty, string.Empty, string.Empty, string.Empty);
}