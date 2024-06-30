using Inc.TeamAssistant.WebUI.Features.Dashboard;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal static class DateSelectorDataFactory
{
    public static IReadOnlyCollection<DateSelectorItem> Create(Dictionary<string, string> resources)
    {
        ArgumentNullException.ThrowIfNull(resources);
        
        return new[]
        {
            new DateSelectorItem(
                resources[Messages.Dashboard_OneWeek],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-7).Date)),
            new DateSelectorItem(
                resources[Messages.Dashboard_TwoWeeks],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-14).Date)),
            new DateSelectorItem(
                resources[Messages.Dashboard_FourWeeks],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-28).Date)),
            new DateSelectorItem(
                resources[Messages.Dashboard_TwelveWeeks],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-84).Date))
        };
    }
}