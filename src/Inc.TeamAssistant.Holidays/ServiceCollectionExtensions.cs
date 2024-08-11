using Inc.TeamAssistant.Holidays.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Holidays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolidays(this IServiceCollection services, TimeSpan cacheAbsoluteExpiration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<HolidayReader>()
            .AddSingleton<IHolidayReader>(sp => ActivatorUtilities.CreateInstance<HolidayReaderCache>(
                sp,
                sp.GetRequiredService<HolidayReader>(),
                cacheAbsoluteExpiration))
            .AddSingleton<IHolidayService, HolidayService>();

        return services;
    }
}