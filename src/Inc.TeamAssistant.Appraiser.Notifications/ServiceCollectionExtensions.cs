using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using Inc.TeamAssistant.Appraiser.Notifications.Builders;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.Notifications;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddNotifications(
		this IServiceCollection services,
        string setCommand,
		string noIdeaCommand)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(setCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(setCommand));
		if (string.IsNullOrWhiteSpace(noIdeaCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(noIdeaCommand));

        services
            .AddScoped(sp => ActivatorUtilities.CreateInstance<SummaryByStoryBuilder>(sp, setCommand, noIdeaCommand));

		services
            .AddScoped<INotificationBuilder<ChangeLanguageResult>, ChangeLanguageNotificationBuilder>()
			.AddScoped<INotificationBuilder<ActivateAssessmentResult>, ActivateAssessmentNotificationBuilder>()
			.AddScoped<INotificationBuilder<ConnectToDashboardResult>, ConnectToDashboardNotificationBuilder>()
			.AddScoped<INotificationBuilder<AddStoryToAssessmentSessionResult>, AddStoryToAssessmentSessionNotificationBuilder>()
			.AddScoped<INotificationBuilder<ConnectToAssessmentSessionResult>, ConnectToAssessmentSessionNotificationBuilder>()
			.AddScoped<INotificationBuilder<CreateAssessmentSessionResult>, CreateAssessmentSessionNotificationBuilder>()
            .AddScoped<INotificationBuilder<FinishAssessmentSessionResult>, FinishAssessmentSessionNotificationBuilder>()
			.AddScoped<INotificationBuilder<AcceptEstimateResult>, AcceptEstimateNotificationBuilder>()
			.AddScoped<INotificationBuilder<SetEstimateForStoryResult>, SetEstimateForStoryNotificationBuilder>()
			.AddScoped<INotificationBuilder<ReVoteEstimateResult>, ReVoteEstimateNotificationBuilder>()
			.AddScoped<INotificationBuilder<ShowParticipantsResult>, ShowParticipantsNotificationBuilder>()
			.AddScoped<INotificationBuilder<StartStorySelectionResult>, StartStorySelectionNotificationBuilder>()
            .AddScoped<INotificationBuilder<ExitFromAssessmentSessionResult>, ChangeUserNotificationBuilder>()
			.AddScoped<INotificationBuilder<AddStoryForEstimateResult>, ChangeUserNotificationBuilder>()
            .AddScoped<INotificationBuilder<AllowUseNameResult>, ChangeUserNotificationBuilder>()
            .AddScoped<INotificationBuilder<ShowHelpResult>>(
                sp => ActivatorUtilities.CreateInstance<ShowHelpNotificationBuilder>(sp, noIdeaCommand))
            .AddScoped<INotificationBuilder<JoinToAssessmentSessionResult>, JoinToAssessmentSessionNotificationBuilder>();

		return services;
	}
}