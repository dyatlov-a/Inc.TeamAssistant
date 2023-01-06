namespace Inc.TeamAssistant.Appraiser.Backend.Pages.Models;

public sealed record MetaStaticViewModel(string Title, string Description, string Keywords, string Author)
{
    public static readonly MetaStaticViewModel Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}