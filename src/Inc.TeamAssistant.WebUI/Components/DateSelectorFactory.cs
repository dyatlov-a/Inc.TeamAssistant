using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Components;

internal sealed class DateSelectorFactory
{
    private readonly IStringLocalizer<ComponentResources> _localizer;

    public DateSelectorFactory(IStringLocalizer<ComponentResources> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public IReadOnlyDictionary<string, DateOnly> CreateWeeks()
    {
        return new Dictionary<string, DateOnly>
        {
            [_localizer["TwoWeeks"].Value] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-14).Date),
            [_localizer["FourWeeks"].Value] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-28).Date),
            [_localizer["TwelveWeeks"].Value] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-84).Date)
        };
    }
    
    public IReadOnlyDictionary<string, DateOnly> CreateMonths()
    {
        return new Dictionary<string, DateOnly>
        {
            [_localizer["OneMonth"].Value] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-1).Date),
            [_localizer["ThreeMonths"].Value] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-3).Date),
            [_localizer["SixMonths"].Value] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-6).Date)
        };
    }
}