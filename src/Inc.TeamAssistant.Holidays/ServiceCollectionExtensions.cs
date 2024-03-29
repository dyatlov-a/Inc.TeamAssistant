using Inc.TeamAssistant.Holidays.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Holidays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolidays(
        this IServiceCollection services,
        WorkdayOptions options,
        TimeSpan cacheAbsoluteExpiration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton(options)
            .AddSingleton<HolidayReader>()
            .AddSingleton<IHolidayReader>(sp => ActivatorUtilities.CreateInstance<HolidayReaderCache>(
                sp,
                sp.GetRequiredService<HolidayReader>(),
                cacheAbsoluteExpiration))
            .AddSingleton<IHolidayService, HolidayService>();

        return services;
    }
}