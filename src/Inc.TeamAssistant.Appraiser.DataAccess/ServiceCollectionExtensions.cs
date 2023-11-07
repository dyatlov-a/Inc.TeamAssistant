using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserDataAccess(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<IAssessmentSessionRepository, AssessmentSessionInMemoryRepository>();

        return services;
    }
}