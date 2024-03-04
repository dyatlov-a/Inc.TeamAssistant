using Inc.TeamAssistant.Connector.Application.CommandHandlers.ChangeTeamProperty.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorApplication(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<PersonService>()
            .AddSingleton<CommandFactory>()
            .AddSingleton<DialogCommandFactory>()
            .AddSingleton<MessageContextBuilder>()
            .AddSingleton<DialogContinuation>()
            .AddSingleton<TelegramBotMessageHandler>()
            .AddHostedService<TelegramBotConnector>()
            
            .AddSingleton<ICommandCreator, CreateTeamCommandCreator>()
            .AddSingleton<ICommandCreator, EndCommandCreator>()
            .AddSingleton<ICommandCreator, HelpCommandCreator>()
            .AddSingleton<ICommandCreator, JoinToTeamCommandCreator>()
            .AddSingleton<ICommandCreator, LeaveFromTeamCommandCreator>()
            
            .AddSingleton<ICommandCreator>(sp => ActivatorUtilities.CreateInstance<ChangeTeamPropertyCommandCreator>(
                sp,
                "/move_to_sp",
                "storyType",
                "Scrum"))
            .AddSingleton<ICommandCreator>(sp => ActivatorUtilities.CreateInstance<ChangeTeamPropertyCommandCreator>(
                sp,
                "/move_to_tshirts",
                "storyType",
                "Kanban"));
        
        return services;
    }
}