using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.DataAccess.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryDataAccess(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<IAssessmentSessionRepository, AssessmentSessionInMemoryRepository>()
            .AddSingleton<IDialogContinuation, DialogContinuation>();

        return services;
    }
}