using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Services;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;
using Inc.TeamAssistant.RandomCoffee.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.RandomCoffee.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRandomCoffeeApplication(
        this IServiceCollection services,
        RandomCoffeeOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton(options)
            .AddSingleton<NotificationsBuilder>()
            
            .AddSingleton<ICommandCreator, InviteForCoffeeCommandCreator>()
            .AddSingleton<ICommandCreator, AddPollAnswerCommandCreator>()
            
            .AddHostedService<ScheduleService>();

        return services;
    }
}