using Dapper;
using Inc.TeamAssistant.Reviewer.All.Holidays.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.All.Holidays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolidays(
        this IServiceCollection services,
        string connectionString,
        HolidayOptions options)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        services
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<HolidayReader>(sp, connectionString))
            .AddSingleton<IHolidayReader>(sp => ActivatorUtilities.CreateInstance<HolidayReaderCache>(
                sp,
                sp.GetRequiredService<HolidayReader>(),
                options.CacheTimeout))
            .AddSingleton<IHolidayService, HolidayService>();

        return services;
    }
}