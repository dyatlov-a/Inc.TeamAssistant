using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.ExceptionHandlers;
using Microsoft.AspNetCore.DataProtection;
using Serilog;
using Serilog.Events;
using Prometheus.DotNetRuntime;

namespace Inc.TeamAssistant.Gateway.Services;

public static class WebApplicationBuilderExtensions
{
    private const string DefaultOutputTemplate =
        "[{Timestamp:yyyy.MM.ddTHH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    
    public static WebApplicationBuilder ConfigureTelemetry(
        this WebApplicationBuilder builder,
        LogEventLevel minLogLevel = LogEventLevel.Warning,
        string outputTemplate = DefaultOutputTemplate)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputTemplate);
        
        var analyticsOptions = builder.Configuration
            .GetSection(nameof(AnalyticsOptions))
            .Get<AnalyticsOptions>() ?? new AnalyticsOptions();
        
        DotNetRuntimeStatsBuilder
            .Customize()
            .WithGcStats()
            .WithThreadPoolStats()
            .StartCollecting();

        builder.Services
            .AddSingleton(analyticsOptions)
            .AddProblemDetails()
            .AddExceptionHandler<TeamAssistantUserExceptionHandler>()
            .AddExceptionHandler<ValidationExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>();
        
        builder.Host.UseSerilog((_, c) =>
        {
            c.Enrich.FromLogContext();
            c.MinimumLevel.Is(minLogLevel);

            if (string.IsNullOrWhiteSpace(analyticsOptions.SentryDsn))
                c.WriteTo.Console(minLogLevel, outputTemplate);
            else
                c.WriteTo.Console(minLogLevel, outputTemplate).WriteTo.Sentry(s =>
                {
                    s.Environment = builder.Environment.EnvironmentName;
                    s.Dsn = analyticsOptions.SentryDsn;
                    s.MinimumBreadcrumbLevel = LogEventLevel.Warning;
                    s.MinimumEventLevel = LogEventLevel.Error;
                });
        });
        
        return builder;
    }

    public static WebApplicationBuilder ConfigureDataProtection(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        var keysDirectory = builder.Environment.IsDevelopment()
            ? Path.Combine(builder.Environment.WebRootPath, "teamassist/keys")
            : "/teamassist/keys";
        
        builder.Services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory));

        return builder;
    }
}