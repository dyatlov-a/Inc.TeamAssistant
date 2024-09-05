using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Features.Components;

internal sealed class DateSelectorFactory
{
    private readonly ResourcesManager _resources;

    public DateSelectorFactory(ResourcesManager resources)
    {
        _resources = resources ?? throw new ArgumentNullException(nameof(resources));
    }

    public IReadOnlyCollection<SelectItem<DateOnly>> CreateShortPeriods()
    {
        return new[]
        {
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_OneWeek],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-7).Date)),
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_TwoWeeks],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-14).Date)),
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_FourWeeks],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-28).Date)),
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_TwelveWeeks],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(-84).Date))
        };
    }
    
    public IReadOnlyCollection<SelectItem<DateOnly>> CreateLongPeriods()
    {
        return new[]
        {
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_OneMonth],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-1).Date)),
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_ThreeMonths],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-3).Date)),
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_SixMonths],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-6).Date)),
            new SelectItem<DateOnly>(
                _resources[Messages.Dashboard_OneYear],
                DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddMonths(-12).Date))
        };
    }
}