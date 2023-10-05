using Inc.TeamAssistant.DialogContinuations.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.DialogContinuations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogContinuations(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(typeof(IDialogContinuation<>), typeof(DialogContinuation<>));

        return services;
    }
}