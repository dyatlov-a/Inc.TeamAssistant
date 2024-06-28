namespace Inc.TeamAssistant.WebUI.Features.Common;

public sealed record MainFooterViewModel(Dictionary<string, IReadOnlyCollection<MainFooterViewModel.FooterLink>> Links)
    : IViewModel<MainFooterViewModel>
{
    public static MainFooterViewModel Empty { get; } = new(new Dictionary<string, IReadOnlyCollection<FooterLink>>());

    public sealed record FooterLink(string Title, string Url, bool External);
}