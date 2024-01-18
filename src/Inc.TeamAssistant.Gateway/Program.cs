using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.DataAccess;
using Inc.TeamAssistant.WebUI.Services;
using Inc.TeamAssistant.CheckIn.Application;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.DataAccess;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.Connector.Application;
using Inc.TeamAssistant.Connector.DataAccess;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Gateway;
using Inc.TeamAssistant.Gateway.Hubs;
using Inc.TeamAssistant.Gateway.PipelineBehaviors;
using Inc.TeamAssistant.Gateway.Services;
using Inc.TeamAssistant.Gateway.Services.CheckIn;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.DataAccess;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Prometheus;
using Prometheus.DotNetRuntime;
using Inc.TeamAssistant.Connector.Application.Contracts;

var builder = WebApplication.CreateBuilder(args);

var telegramBotOptions = builder.Configuration.GetRequiredSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>()!;
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var checkInOptions = builder.Configuration.GetRequiredSection(nameof(CheckInOptions)).Get<CheckInOptions>()!;
var reviewerOptions = builder.Configuration.GetRequiredSection(nameof(ReviewerOptions)).Get<ReviewerOptions>()!;
var holidayOptions = builder.Configuration.GetRequiredSection(nameof(HolidayOptions)).Get<HolidayOptions>()!;
var addStoryOptions = builder.Configuration.GetRequiredSection(nameof(AddStoryOptions)).Get<AddStoryOptions>()!;

builder.Services
	.AddMediatR(c =>
	{
		c.Lifetime = ServiceLifetime.Scoped;
		c.RegisterServicesFromAssemblyContaining<IStoryRepository>();
		c.RegisterServicesFromAssemblyContaining<ILocationsRepository>();
		c.RegisterServicesFromAssemblyContaining<ITaskForReviewRepository>();
		c.RegisterServicesFromAssemblyContaining<ITeamRepository>();
	})
	
	.AddScoped<ITranslateProvider, TranslateProvider>()
	.AddScoped<ICheckInService, CheckInService>()
	.AddScoped<ILocationBuilder, DummyLocationBuilder>()
	.AddHolidays(connectionString, holidayOptions)
		
    .AddAppraiserApplication(addStoryOptions)
    .AddAppraiserDataAccess(connectionString)
	
    .AddCheckInApplication(checkInOptions)
    .AddCheckInDataAccess(connectionString)
	
    .AddReviewerApplication(reviewerOptions)
	.AddReviewerDataAccess(connectionString)
	
	.AddConnectorApplication()
	.AddConnectorDataAccess(connectionString)
	
	.AddMemoryCache()
	.AddServices(telegramBotOptions, builder.Environment.WebRootPath)
    .AddIsomorphic()
    .AddMvc();

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

ValidatorOptions.Global.LanguageManager.Culture = new("en");
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

builder.Services
	.AddValidatorsFromAssemblyContaining<IStoryRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITaskForReviewRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITeamRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.TryAddEnumerable(ServiceDescriptor.Scoped(
		typeof(IPipelineBehavior<,>),
		typeof(ValidationPipelineBehavior<,>)));

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