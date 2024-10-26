using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Services;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.RefuseForCoffee.Services;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;
using Inc.TeamAssistant.RandomCoffee.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.RandomCoffee.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRandomCoffeeApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ISettingSectionProvider, RandomCoffeeSettingSectionProvider>()
                
            .AddSingleton<NotificationsBuilder>()
            
            .AddSingleton<ICommandCreator, InviteForCoffeeCommandCreator>()
            .AddSingleton<ICommandCreator, AddPollAnswerCommandCreator>()
            .AddSingleton<ICommandCreator, RefuseForCoffeeCommandCreator>()
            
            .AddHostedService<ScheduleService>();

        return services;
    }
}