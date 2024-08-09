namespace Inc.TeamAssistant.Gateway.Configs;

public sealed class AnalyticsOptions
{
    public string? SentryDsn { get; set; }
    public string? GoogleVerificationValue { get; set; }
    public string? YandexVerificationValue { get; set; }
}