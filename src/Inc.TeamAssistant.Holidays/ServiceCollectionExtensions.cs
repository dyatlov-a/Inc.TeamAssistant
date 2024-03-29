using Dapper;
using Inc.TeamAssistant.Holidays.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Holidays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolidays(
        this IServiceCollection services,
        string connectionString,
        WorkdayOptions options,
        TimeSpan cacheAbsoluteExpiration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new DateTimeOffsetTypeHandler());

        services
            .AddSingleton(options)
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<HolidayReader>(sp, connectionString))
            .AddSingleton<IHolidayReader>(sp => ActivatorUtilities.CreateInstance<HolidayReaderCache>(
                sp,
                sp.GetRequiredService<HolidayReader>(),
                cacheAbsoluteExpiration))
            .AddSingleton<IHolidayService, HolidayService>();

        return services;
    }
}