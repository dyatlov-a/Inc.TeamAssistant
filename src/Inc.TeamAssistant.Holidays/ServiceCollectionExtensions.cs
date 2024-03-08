using Dapper;
using Inc.TeamAssistant.Holidays.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Holidays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolidays(
        this IServiceCollection services,
        string connectionString,
        TimeSpan cacheAbsoluteExpiration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        services
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<HolidayReader>(sp, connectionString))
            .AddSingleton<IHolidayReader>(sp => ActivatorUtilities.CreateInstance<HolidayReaderCache>(
                sp,
                sp.GetRequiredService<HolidayReader>(),
                cacheAbsoluteExpiration))
            .AddSingleton<IHolidayService, HolidayService>();

        return services;
    }
}