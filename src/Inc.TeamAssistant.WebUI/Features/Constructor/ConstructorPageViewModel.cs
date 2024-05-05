namespace Inc.TeamAssistant.WebUI.Features.Constructor;

public sealed record ConstructorPageViewModel(string SelectBotText)
{
    public static readonly ConstructorPageViewModel Empty = new(string.Empty);
}