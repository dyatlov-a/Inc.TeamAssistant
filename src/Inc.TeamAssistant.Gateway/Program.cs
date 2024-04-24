using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application;
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
using Inc.TeamAssistant.Gateway.Hubs;
using Inc.TeamAssistant.Gateway.PipelineBehaviors;
using Inc.TeamAssistant.Gateway.Services;
using Inc.TeamAssistant.Gateway.Services.CheckIn;
using Inc.TeamAssistant.Reviewer.Application;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.DataAccess;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Prometheus;
using Prometheus.DotNetRuntime;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.RandomCoffee.Application;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.DataAccess;
using Inc.TeamAssistant.RandomCoffee.Domain;
using MediatR.Pipeline;

var builder = WebApplication.CreateBuilder(args);

var cacheAbsoluteExpiration = builder.Configuration.GetRequiredSection("CacheAbsoluteExpiration").Get<TimeSpan>();
var appraiserOptions = builder.Configuration.GetRequiredSection(nameof(AppraiserOptions)).Get<AppraiserOptions>()!;
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var checkInOptions = builder.Configuration.GetRequiredSection(nameof(CheckInOptions)).Get<CheckInOptions>()!;
var reviewerOptions = builder.Configuration.GetRequiredSection(nameof(ReviewerOptions)).Get<ReviewerOptions>()!;
var workdayOptions = builder.Configuration.GetRequiredSection(nameof(WorkdayOptions)).Get<WorkdayOptions>()!;
var randomCoffeeOptions = builder.Configuration.GetRequiredSection(nameof(RandomCoffeeOptions)).Get<RandomCoffeeOptions>()!;

builder.Services
	.AddDataAccess(connectionString)
	.AddMessageIdType()
	.AddLanguageIdType()
	.AddDateOnlyType()
	.AddDateTimeOffsetType()
	.AddJsonType<ICollection<string>>()
	.AddJsonType<ICollection<long>>()
	.AddJsonType<ICollection<PersonPair>>()
	.AddJsonType<IReadOnlyDictionary<string, string>>();

builder.Services
	.AddMediatR(c =>
	{
		c.Lifetime = ServiceLifetime.Scoped;
		c.RegisterServicesFromAssemblyContaining<IStoryRepository>();
		c.RegisterServicesFromAssemblyContaining<ILocationsRepository>();
		c.RegisterServicesFromAssemblyContaining<ITaskForReviewRepository>();
		c.RegisterServicesFromAssemblyContaining<ITeamRepository>();
		c.RegisterServicesFromAssemblyContaining<IRandomCoffeeRepository>();
	})
	.AddValidatorsFromAssemblyContaining<IStoryRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ILocationsRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITaskForReviewRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITeamRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<IRandomCoffeeRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>))
	.TryAddEnumerable(ServiceDescriptor.Scoped(
		typeof(IPipelineBehavior<,>),
		typeof(ValidationPipelineBehavior<,>)));

builder.Services
	.AddScoped<ICheckInService, CheckInService>()
	.AddScoped<ILocationBuilder, DummyLocationBuilder>()
	.AddHolidays(workdayOptions, cacheAbsoluteExpiration)
		
    .AddAppraiserApplication(appraiserOptions)
    .AddAppraiserDataAccess()
	
    .AddCheckInApplication(checkInOptions)
    .AddCheckInDataAccess()
	
    .AddReviewerApplication(reviewerOptions)
	.AddReviewerDataAccess()
	
	.AddRandomCoffeeApplication(randomCoffeeOptions)
	.AddRandomCoffeeDataAccess()
	
	.AddConnectorApplication()
	.AddConnectorDataAccess(cacheAbsoluteExpiration)
	
	.AddMemoryCache()
	.AddHttpContextAccessor()
	.AddServices(builder.Environment.WebRootPath, cacheAbsoluteExpiration)
    .AddIsomorphic()
    .AddMvc();

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

ValidatorOptions.Global.LanguageManager.Culture = new(LanguageSettings.DefaultLanguageId.Value);
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

var app = builder.Build();

if (builder.Environment.IsDevelopment())
	app.UseWebAssemblyDebugging();

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