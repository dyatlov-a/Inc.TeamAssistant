using Inc.TeamAssistant.Survey.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Survey.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSurveyDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ISurveyReader, SurveyReader>()
            
            .AddSingleton<ISurveyRepository, SurveyRepository>();
        
        return services;
    }
}