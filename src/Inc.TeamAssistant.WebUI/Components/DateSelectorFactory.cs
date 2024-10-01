using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Components;

internal sealed class DateSelectorFactory
{
    private readonly ResourcesManager _resources;

    public DateSelectorFactory(ResourcesManager resources)
    {
        _resources = resources ?? throw new ArgumentNullException(nameof(resources));
    }

    public IReadOnlyDictionary<string, DateOnly> CreateWeeks()
    {
        return new Dictionary<string, DateOnly>
        {
            [_resources[Messages.Dashboard_TwoWeeks]] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-14).Date),
            [_resources[Messages.Dashboard_FourWeeks]] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-28).Date),
            [_resources[Messages.Dashboard_TwelveWeeks]] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-84).Date)
        };
    }
    
    public IReadOnlyDictionary<string, DateOnly> CreateMonths()
    {
        return new Dictionary<string, DateOnly>
        {
            [_resources[Messages.Dashboard_OneMonth]] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-1).Date),
            [_resources[Messages.Dashboard_ThreeMonths]] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-3).Date),
            [_resources[Messages.Dashboard_SixMonths]] = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-6).Date)
        };
    }
}