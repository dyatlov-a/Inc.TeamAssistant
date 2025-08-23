using Inc.TeamAssistant.Survey.Application.QueryHandlers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Survey.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSurveyApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<SurveySummaryService>();
        
        return services;
    }
}