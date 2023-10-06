using Inc.TeamAssistant.Appraiser.Application;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Backend;
using Inc.TeamAssistant.Appraiser.Backend.Hubs;
using Inc.TeamAssistant.Appraiser.Backend.Services;
using Inc.TeamAssistant.Appraiser.Backend.Services.CheckIn;
using Inc.TeamAssistant.Appraiser.DataAccess;
using Inc.TeamAssistant.WebUI.Services;
using Inc.TeamAssistant.Appraiser.Notifications;
using Inc.TeamAssistant.CheckIn.Application;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.DataAccess;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Users;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Reviewer.Application;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.DataAccess;
using Prometheus;
using Prometheus.DotNetRuntime;

var builder = WebApplication.CreateBuilder(args);

var telegramBotOptions = builder.Configuration.GetRequiredSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>()!;
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var checkInOptions = builder.Configuration.GetRequiredSection(nameof(CheckInOptions)).Get<CheckInOptions>()!;
var reviewerOptions = builder.Configuration.GetRequiredSection(nameof(ReviewerOptions)).Get<ReviewerOptions>()!;
var holidayOptions = builder.Configuration.GetRequiredSection(nameof(HolidayOptions)).Get<HolidayOptions>()!;

builder.Services
	.AddMediatR(c =>
	{
		c.Lifetime = ServiceLifetime.Scoped;
		c.RegisterServicesFromAssemblyContaining<IAssessmentSessionRepository>();
		c.RegisterServicesFromAssemblyContaining<ILocationsRepository>();
		c.RegisterServicesFromAssemblyContaining<ITeamRepository>();
	})
	.AddScoped<ITranslateProvider, TranslateProvider>()
		
    .AddApplication(builder.Configuration)
    .AddAppraiserDataAccess(connectionString, UserSettings.AnonymousUser)
	.AddNotifications()
	.AddServices(telegramBotOptions, builder.Environment.WebRootPath)

    .AddScoped<ICheckInService, CheckInService>()
    .AddScoped<ILocationBuilder, DummyLocationBuilder>()
    .AddCheckInApplication(checkInOptions)
    .AddCheckInDataAccess(connectionString)
	
    .AddReviewer(reviewerOptions)
	.AddReviewerDataAccess(connectionString)
    .AddMemoryCache()
    .AddHolidays(connectionString, holidayOptions)
    .AddDialogContinuations()

    .AddIsomorphic()
    .AddMvc();

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}

app
	.UseStaticFiles()
	.UseBlazorFrameworkFiles()
	.UseRouting()
	.UseEndpoints(e =>
	{
		e.MapDefaultControllerRoute();
		e.MapFallbackToPage("/_Host");
        e.MapMetrics();
        e.MapHealthChecks("/health");
    });

app.MapHub<MessagesHub>("/messages");;

DotNetRuntimeStatsBuilder
    .Customize()
    .WithGcStats()
    .WithThreadPoolStats()
    .StartCollecting();

app.Run();