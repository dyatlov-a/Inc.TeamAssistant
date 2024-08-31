using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.ExceptionHandlers;
using Serilog;
using Serilog.Events;
using Prometheus.DotNetRuntime;

namespace Inc.TeamAssistant.Gateway.Services;

public static class WebApplicationBuilderExtensions
{
    private const string DefaultOutputTemplate =
        "[{Timestamp:yyyy.MM.ddTHH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    
    public static WebApplicationBuilder UseTelemetry(
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
            .AddExceptionHandler<UnhandledExceptionHandler>();
        
        builder.Host.UseSerilog((_, c) =>
        {
            c.Enrich.FromLogContext();
            c.MinimumLevel.Is(minLogLevel);
            c.WriteTo.Console(minLogLevel, outputTemplate);

            if (!string.IsNullOrWhiteSpace(analyticsOptions.SentryDsn))
                c.WriteTo.Sentry(s =>
                {
                    s.Environment = builder.Environment.EnvironmentName;
                    s.Dsn = analyticsOptions.SentryDsn;
                    s.MinimumBreadcrumbLevel = LogEventLevel.Warning;
                    s.MinimumEventLevel = LogEventLevel.Error;
                });
        });
        
        return builder;
    }
}