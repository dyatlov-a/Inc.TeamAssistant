using Inc.TeamAssistant.Appraiser.Application;
using Inc.TeamAssistant.Appraiser.Backend;
using Inc.TeamAssistant.Appraiser.Backend.Hubs;
using Inc.TeamAssistant.Appraiser.Backend.Services;
using Inc.TeamAssistant.Appraiser.Backend.Services.CheckIn;
using Inc.TeamAssistant.Appraiser.Dashboard;
using Inc.TeamAssistant.Appraiser.Dashboard.Services;
using Inc.TeamAssistant.Appraiser.DataAccess.InMemory;
using Inc.TeamAssistant.Appraiser.DataAccess.Postgres;
using Inc.TeamAssistant.Appraiser.Model.CheckIn;
using Inc.TeamAssistant.Appraiser.Notifications;
using Inc.TeamAssistant.CheckIn.All;
using Inc.TeamAssistant.Reviewer.All;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations;
using Inc.TeamAssistant.Reviewer.All.Holidays;
using Prometheus;
using Prometheus.DotNetRuntime;

var builder = WebApplication.CreateBuilder(args);

var telegramBotOptions = builder.Configuration.GetSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>();
var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var checkInOptions = builder.Configuration.GetSection(nameof(CheckInOptions)).Get<CheckInOptions>();
var reviewerOptions = builder.Configuration.GetSection(nameof(ReviewerOptions)).Get<ReviewerOptions>();
var holidayOptions = builder.Configuration.GetSection(nameof(HolidayOptions)).Get<HolidayOptions>();

builder.Services
    .AddApplication(builder.Configuration)
    .AddInMemoryDataAccess()
    .AddPostgresDataAccess(connectionString, Settings.AnonymousUser)
	.AddNotifications(CommandList.SetEstimateForStory, CommandList.NoIdea)
	.AddServices(telegramBotOptions, builder.Environment.WebRootPath)

    .AddSingleton<ICheckInService, CheckInService>()
    .AddScoped<ILocationBuilder, DummyLocationBuilder>()
    .AddScoped<Inc.TeamAssistant.CheckIn.All.Contracts.ITranslateProvider, TranslateProvider>()
    .AddCheckIn(checkInOptions, connectionString)

    .AddScoped<Inc.TeamAssistant.Reviewer.All.Contracts.ITranslateProvider, TranslateProvider>()
    .AddReviewer(reviewerOptions, connectionString)
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